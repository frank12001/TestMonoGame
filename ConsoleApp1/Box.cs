using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Box
    {
        Model model;
        float speed = 1;
        Vector3 position=Vector3.Zero;
        Vector3 rotation = Vector3.Zero;
        Vector3 scale = Vector3.One;
        public void Initialize(ContentManager contentManager)
        {
            model = contentManager.Load<Model>("box1");
        }
        public void Update(GameTime gameTime)
        {
            position += Vector3.Backward * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Console.WriteLine(position); 
            //往下是 y++
            //往左是 x++
            //往上是 z++
        }

        public void Draw(Camera camera,GraphicsDevice graphicsDevice)
        {
            var texture2D = new Texture2D(graphicsDevice, 1, 1);
            texture2D.SetData(new[] { Color.Red });
            foreach (var mesh in model.Meshes)
            {                
                foreach (BasicEffect effect in mesh.Effects)
                {                    
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.Texture = texture2D;
                    effect.TextureEnabled = true;

                    effect.World = GetWorldMatrix();                 
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }

                mesh.Draw();
            }
        }
        Matrix GetWorldMatrix()
        {
            //const float circleRadius = 8;
            //const float heightOffGround = 3;

            //// this matrix moves the model "out" from the origin
            //Matrix translationMatrix = Matrix.CreateTranslation(
            //    circleRadius, 0, heightOffGround);

            //// this matrix rotates everything around the origin
            //Matrix rotationMatrix = Matrix.CreateRotationZ(angle);

            //// We combine the two to have the model move in a circle:
            //Matrix combined = translationMatrix * rotationMatrix;

            //return combined;

            return Matrix.CreateTranslation(position) 
                *Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z)
                * Matrix.CreateScale(scale);
        }
    }
}
