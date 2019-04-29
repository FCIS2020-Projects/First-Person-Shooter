using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GlmNet;

namespace Graphics
{
    class bullet
    {
        Model3D bulletModel;
        vec3 shootPosition;
        vec3 mPosition;
        vec3 mDirection;
        public bullet(Model3D model, Camera cam)
        {
            shootPosition = cam.GetCameraPosition();
            shootPosition.y -= 1.5f;
            shootPosition += cam.GetLookDirection() * 8;

            bulletModel = model;
            mPosition = shootPosition;
            mDirection = cam.GetLookDirection();
            bulletModel.transmatrix = glm.translate(new mat4(1), mPosition);
            bulletModel.rotmatrix = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.rotate((float)((90.0 / 180.0) * Math.PI), new vec3(1, 0, 0)),
                glm.rotate( -cam.mAngleY, new vec3(1, 0, 0)),
                glm.rotate( cam.mAngleX, new vec3(0, 1, 0))
            });
        }
        public void Draw(int matID)
        {
            bulletModel.Draw(matID);
        }
        public void Update()
        {
            mPosition += mDirection;
            bulletModel.transmatrix = glm.translate(new mat4(1), mPosition);
        }

        public bool shouldBeDestroyed()
        {
            vec3 distance = shootPosition - mPosition;
            if(distance.x > 100 || distance.y > 100 || distance.z > 100 ||
                distance.x < -100 || distance.y < -100 || distance.z < -100)
                return true;
            return false;
        }
    }
}
