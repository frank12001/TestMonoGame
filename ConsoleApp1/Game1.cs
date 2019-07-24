using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace ConsoleApp1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        CollisionSystem collision;
        World world;


        Shape shape1;
        RigidBody body1;
        BoxPrimitive Box1;

        BoxShape shape2;
        RigidBody body2;
        BoxPrimitive Box2;

        BoxShape shape0;
        RigidBody body0;
        BoxPrimitive Box0;

        Texture2D checkerboardTexture;
        Texture2D whiteRectangle;

        BasicEffect effect;

        Robot robot;       
        Camera Camera;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            collision = new CollisionSystemSAP();
            collision.CollisionDetected += Collision_CollisionDetected;
            world = new World(collision);

            //while (true)
            //{
            //    world.Step(0.001f, true);

                //bool result = world.CollisionSystem.Raycast(JVector.Zero, JVector.Up * 2, RaycastCallback, out RigidBody resBody, out JVector hitNormal, out float fraction);

                //if (result)
                //{
                //    JVector hitPoint = JVector.Zero + fraction * (JVector.Up * 2);
                //    Console.WriteLine($"Ray Cast!! Hit Point: {hitPoint.ToString()} Body Name: {resBody.Tag}");
                //}

                ////collision.Detect(body, body2);

                //Console.WriteLine($"body1 : {body.position.X},{body.position.Y},{body.position.Z}");
                ////Console.WriteLine($"body2 : {body2.position.X},{body2.position.Y},{body2.position.Z}");

                //Thread.Sleep(1000);
            //}

            //bool RaycastCallback(RigidBody body, JVector normal, float fraction)
            //{
            //    //return !body.IsStatic;  撞到的目標不是 Static 才會算   

            //    return body.IsStatic;
            //}
        }

        private void Collision_CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            if ((body1.Tag.ToString() == "body1" || body2.Tag.ToString() == "body1") && !body1.IsStatic && !body2.IsStatic)
            {
                Console.WriteLine($"body1 碰撞");
            }
        }

        protected override void Initialize()
        {
            shape1 = new BoxShape(JVector.One);
            body1 = new RigidBody(shape1) { Tag = "body1", Position = JVector.Left * 5 };
            Box1 = new BoxPrimitive(GraphicsDevice);
            world.AddBody(body1);

            shape2 = new BoxShape(JVector.One);
            body2 = new RigidBody(shape2) { Tag = "body2", Position = JVector.Up*6 };
            Box2 = new BoxPrimitive(GraphicsDevice);
            world.AddBody(body2);

            var scale = 40;
            shape0 = new BoxShape(JVector.One*scale);
            body0 = new RigidBody(shape0) { Tag = "body0", Position = JVector.Down*(scale/2),IsStatic = true };
            Box0 = new BoxPrimitive(GraphicsDevice,scale);
            world.AddBody(body0);

            robot = new Robot();
            robot.Initialize(Content);
              
            effect = new BasicEffect(graphics.GraphicsDevice);

            Camera = new Camera(this);
            Camera.Position = new Vector3(0, 5, 20);
            Camera.Target = Camera.Position + Vector3.Normalize(new Vector3(0,5, 20));
            this.Components.Add(Camera);

            base.Initialize();
        }

        protected override void LoadContent()
        {       
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //whiteRectangle = Content.Load<Texture2D>("60789_01");
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            // We aren't using the content pipeline, so we need
            // to access the stream directly:
            using (var stream = TitleContainer.OpenStream("Content/checkerboard.png"))
            {
                checkerboardTexture = Texture2D.FromStream(this.GraphicsDevice, stream);
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            spriteBatch.Dispose();
            whiteRectangle.Dispose();
        }
        protected override void Update(GameTime gameTime)
        {
            //collision.Detect(body1, body2);            
            bool result = world.CollisionSystem.Raycast(body2.Position, JVector.Left * 100, (body, normal, fraction) => { return body.Tag != body2.Tag; }
                , out RigidBody resBody, out JVector hitNormal, out float fraction);
            if (result)
            {
                Console.WriteLine($"Ray Cast!!Body Name: {resBody.Tag}");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //在 JV 中一個物體太久沒動 IsActive 會設為 false
                body2.IsActive = true;
                body2.Position = JVector.Up * 6;        
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                body2.IsActive = true;
                body2.Position = (JVector.Up * 6) + (JVector.Right * 6);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                body2.IsActive = true;
                body2.Position = (JVector.Up * 6) + (JVector.Left * 6);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                body2.IsActive = true;
                body2.Position = (JVector.Up * 6) + (JVector.Backward * 6);
            }
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, false);
                    
            Box1.AddWorldMatrix(Conversion.ToXNAMatrix(body1.Orientation) * Matrix.CreateTranslation(Conversion.ToXNAVector(body1.Position)));
            Box2.AddWorldMatrix(Conversion.ToXNAMatrix(body2.Orientation) * Matrix.CreateTranslation(Conversion.ToXNAVector(body2.Position)));
            Box0.AddWorldMatrix(Conversion.ToXNAMatrix(body0.Orientation) * Matrix.CreateTranslation(Conversion.ToXNAVector(body0.Position)));

            robot.Update(gameTime);
            //camera.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            robot.Draw(Camera);

            BasicEffect myBoxEffect = new BasicEffect(GraphicsDevice);
            myBoxEffect.EnableDefaultLighting();
            myBoxEffect.PreferPerPixelLighting = true;
            myBoxEffect.View = Camera.View;//camera.ViewMatrix;
            myBoxEffect.Projection = Camera.ViewProjection;//camera.ProjectionMatrix;
            var texture2D = new Texture2D(GraphicsDevice, 1, 1);
            texture2D.SetData(new[] { Color.White });
            myBoxEffect.Texture = texture2D;
            myBoxEffect.TextureEnabled = true;
            Box1.Draw(myBoxEffect);

            texture2D.SetData(new[] { Color.Red });
            myBoxEffect.Texture = texture2D;
            myBoxEffect.TextureEnabled = true;
            Box2.Draw(myBoxEffect);

            texture2D.SetData(new[] { Color.Green });
            myBoxEffect.Texture = texture2D;
            myBoxEffect.TextureEnabled = true;
            Box0.Draw(myBoxEffect);

            //spriteBatch.Begin();
            //spriteBatch.Draw(whiteRectangle, new Rectangle(10, 20, 80, 30), Color.Chocolate);
            //spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
