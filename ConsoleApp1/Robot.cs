using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ConsoleApp1
{
    public class Robot
    {
        Model model;
        float angle;

        public void Initialize(ContentManager contentManager)
        {
            model = contentManager.Load<Model>("robot");           
        }
        public void Update(GameTime gameTime)
        {
            // TotalSeconds is a double so we need to cast to float
            angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(Camera camera)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = GetWorldMatrix();
                    effect.View = camera.View;
                    effect.Projection = camera.ViewProjection;
                }

                mesh.Draw();
            }
        }

        //// For now we'll take these values in, eventually we'll
        //// take a Camera object
        //public void Draw(Vector3 cameraPosition, float aspectRatio)
        //{
        //    foreach (var mesh in model.Meshes)
        //    {
        //        foreach (BasicEffect effect in mesh.Effects)
        //        {
        //            effect.EnableDefaultLighting();
        //            effect.PreferPerPixelLighting = true;

        //            effect.World = GetWorldMatrix();

        //            var cameraLookAtVector = Vector3.Zero;
        //            var cameraUpVector = Vector3.UnitZ;

        //            effect.View = Matrix.CreateLookAt(
        //                cameraPosition, cameraLookAtVector, cameraUpVector);

        //            float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        //            float nearClipPlane = 1;
        //            float farClipPlane = 200;

        //            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
        //                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
        //        }

        //        // Now that we've assigned our properties on the effects we can
        //        // draw the entire mesh
        //        mesh.Draw();
        //    }
        //}
        Matrix GetWorldMatrix()
        {
            const float circleRadius = 8;
            const float heightOffGround = 3;

            // this matrix moves the model "out" from the origin
            Matrix translationMatrix = Matrix.CreateTranslation(
                circleRadius, 0, heightOffGround);

            // this matrix rotates everything around the origin
            Matrix rotationMatrix = Matrix.CreateRotationY(angle);

            // We combine the two to have the model move in a circle:
            Matrix combined = Matrix.CreateRotationX(-90) * Matrix.CreateTranslation(Vector3.Up*2) * translationMatrix * rotationMatrix;

            return combined;
        }
    }
}