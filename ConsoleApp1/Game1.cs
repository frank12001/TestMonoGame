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

        BoxShape shape2;
        RigidBody body2;

        Texture2D checkerboardTexture;
        Texture2D whiteRectangle;

        VertexPositionTexture[] floorVerts;
        BasicEffect effect;

        Robot robot;
        BoxPrimitive Box1;
        BoxPrimitive Box2;

        Camera camera;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            collision = new CollisionSystemSAP();
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
        protected override void Initialize()
        {
            shape1 = new BoxShape(JVector.One);
            body1 = new RigidBody(shape1) { Tag = "body1", Position = JVector.Zero, IsStatic = true };            
            world.AddBody(body1);

            shape2 = new BoxShape(JVector.One);
            body2 = new RigidBody(shape2) { Tag = "body2", Position = JVector.Up*6 };
            world.AddBody(body2);


            floorVerts = new VertexPositionTexture[6];
            floorVerts[0].Position = new Vector3(-20, -20, 0);
            floorVerts[1].Position = new Vector3(-20, 20, 0);
            floorVerts[2].Position = new Vector3(20, -20, 0);
            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(20, 20, 0);
            floorVerts[5].Position = floorVerts[2].Position;            

            int repetitions = 20;

            floorVerts[0].TextureCoordinate = new Vector2(0, 0);
            floorVerts[1].TextureCoordinate = new Vector2(0, repetitions);
            floorVerts[2].TextureCoordinate = new Vector2(repetitions, 0);

            floorVerts[3].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[4].TextureCoordinate = new Vector2(repetitions, repetitions);
            floorVerts[5].TextureCoordinate = floorVerts[2].TextureCoordinate;

            robot = new Robot();
            robot.Initialize(Content);

            Box1 = new BoxPrimitive(GraphicsDevice);

            Box2 = new BoxPrimitive(GraphicsDevice);

            effect = new BasicEffect(graphics.GraphicsDevice);
            camera = new Camera(graphics.GraphicsDevice);

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
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //Box1.Position = (Vector3.Up * 6) + (Vector3.Forward * 6);             
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                //Box1.Position = (Vector3.Up * 6) + (Vector3.Left * 6);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                //Box1.Position = (Vector3.Up * 6) + (Vector3.Right * 6);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //Vector3 v = Vector3.Up * 6;
                //body2.Position = new JVector(v.X, v.Y, v.Z);
                //body1.AddForce(JVector.Up * 100);               
                //Box1.Position = Vector3.Up * 6;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.S))
            {
                
            }
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, false);

            //Box1.Position = Conversion.ToXNAVector(body1.Position);
            //Box2.Position = Conversion.ToXNAVector(body2.Position);
            Box1.AddWorldMatrix(Matrix.CreateTranslation(Conversion.ToXNAVector(body1.Position)));
            Box2.AddWorldMatrix(Matrix.CreateTranslation(Conversion.ToXNAVector(body2.Position)));

            //Console.WriteLine(body2.position);

            robot.Update(gameTime);
            camera.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            robot.Draw(camera);
            //Box1.Draw(camera,GraphicsDevice);
            BasicEffect box3Effect = new BasicEffect(GraphicsDevice);
            box3Effect.View = camera.ViewMatrix;
            box3Effect.Projection = camera.ProjectionMatrix;
            var texture2D = new Texture2D(GraphicsDevice, 1, 1);
            texture2D.SetData(new[] { Color.White });
            box3Effect.Texture = texture2D;
            box3Effect.TextureEnabled = true;
            Box1.Draw(box3Effect);
            texture2D.SetData(new[] { Color.Red });
            box3Effect.Texture = texture2D;
            box3Effect.TextureEnabled = true;
            Box2.Draw(box3Effect);


            //spriteBatch.Begin();
            //spriteBatch.Draw(whiteRectangle, new Rectangle(10, 20, 80, 30), Color.Chocolate);
            //spriteBatch.End();

            //DrawGround();

            base.Draw(gameTime);
        }

        void DrawGround()
        {
            // New camera code
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            effect.TextureEnabled = true;
            effect.Texture = checkerboardTexture;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(
                            PrimitiveType.TriangleList,
                    floorVerts,
                    0,
                    2);
            }
        }
    }
}
