﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Camera
    {
        // We need this to calculate the aspectRatio
        // in the ProjectionMatrix property.
        GraphicsDevice graphicsDevice;

        Vector3 position = new Vector3(0, 20, 10);

        float angle;

        public Matrix ViewMatrix
        {
            get
            {
                var lookAtVector = new Vector3(0, -1, -.5f);
                // We'll create a rotation matrix using our angle
                var rotationMatrix = Matrix.CreateRotationZ(angle);
                // Then we'll modify the vector using this matrix:
                lookAtVector = Vector3.Transform(lookAtVector, rotationMatrix);
                lookAtVector += position;

                var upVector = Vector3.UnitZ;

                return Matrix.CreateLookAt(
                    position, lookAtVector, upVector);
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                float fieldOfView = MathHelper.PiOver4;
                float nearClipPlane = 1;
                float farClipPlane = 200;
                float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(
                    fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Camera(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;           
        }

        public void Update(GameTime gameTime)
        {
            bool isTouchingScreen = Mouse.GetState().LeftButton == ButtonState.Pressed;
            if (isTouchingScreen)
            {
                var xPosition = Mouse.GetState().Position.X;

                float xRatio = xPosition / (float)graphicsDevice.Viewport.Width;

                if (xRatio < 1 / 3.0f)
                {
                    angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (xRatio < 2 / 3.0f)
                {
                    var forwardVector = new Vector3(0, -1, 0);

                    var rotationMatrix = Matrix.CreateRotationZ(angle);
                    forwardVector = Vector3.Transform(forwardVector, rotationMatrix);

                    const float unitsPerSecond = 3;

                    this.position += forwardVector * unitsPerSecond *
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    angle -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }
    }
}
