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

        Texture2D checkerboardTexture;
        Texture2D whiteRectangle;

        Model model_box1;

        VertexPositionTexture[] floorVerts;
        BasicEffect effect;
        Vector3 cameraPosition = new Vector3(15, 10, 10);

        Robot robot;
        Box Box1;

        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            collision = new CollisionSystemSAP();
            world = new World(collision);

            ////BoxShape shape2 = new BoxShape(10000f,1f,10000f);
            ////RigidBody body2 = new RigidBody(shape2) { Tag = "body2" };
            ////world.AddBody(body2);

            //while (true)
            //{
            //    world.Step(0.001f, true);

            //    bool result = world.CollisionSystem.Raycast(JVector.Zero,JVector.Up * 2,RaycastCallback,out RigidBody resBody,out JVector hitNormal,out float fraction);

            //    if (result)
            //    {
            //        JVector hitPoint = JVector.Zero + fraction * (JVector.Up * 2);                    
            //        Console.WriteLine($"Ray Cast!! Hit Point: {hitPoint.ToString()} Body Name: {resBody.Tag}");
            //    }

            //    //collision.Detect(body, body2);

            //    Console.WriteLine($"body1 : {body.position.X},{body.position.Y},{body.position.Z}");
            //    //Console.WriteLine($"body2 : {body2.position.X},{body2.position.Y},{body2.position.Z}");

            //    Thread.Sleep(1000);
            //}

            //bool RaycastCallback(RigidBody body, JVector normal, float fraction)
            //{
            //    //return !body.IsStatic;  撞到的目標不是 Static 才會算   

            //    return body.IsStatic;
            //}
        }
        protected override void Initialize()
        {
            Shape shape = new BoxShape(JVector.One);
            RigidBody body = new RigidBody(shape) { Tag = "body1" };
            body.Position = JVector.Up * 2;
            world.AddBody(body);
            body.IsStatic = true;

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
            Box1 = new Box();
            Box1.Initialize(Content);

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
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds/100f, true);

            robot.Update(gameTime);
            camera.Update(gameTime);
            Box1.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            robot.Draw(camera);
            Box1.Draw(camera,GraphicsDevice);
            //spriteBatch.Begin();
            //spriteBatch.Draw(whiteRectangle, new Rectangle(10, 20, 80, 30), Color.Chocolate);
            //spriteBatch.End();

            DrawGround();

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
