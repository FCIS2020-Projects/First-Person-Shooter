using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using Graphics._3D_Models;
namespace Graphics
{
    class Renderer
    {
        Shader sh;

        uint skyboxBuffer_ID;

        int modelID;
        int viewID;
        int projID;

        int EyePositionID;
        int AmbientLightID;
        int DataID;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;

        public float Speed = 1;


        public Camera cam;

        Model3D building;
        Model3D car;
        Model3D tree;
        
        Model3D spider;
        public md2LOL zombie;
        public md2 Blade;

        Texture d, u, l, r, f, b, groundTexture;
        mat4 down, up, left, right, front, back, ground;

        vec3 playerPos;
        Model3D HandsWGun;
        public Model3D bulletModel;
        Texture shoot;
        float zombieSpeed = 0.4f;
        int Enemy_range_Attack = 50;
        int Enemy_range_Run = 200;

        public List<bullet> bullets = new List<bullet>();

        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            d = new Texture(projectPath + "\\Textures\\skybox\\bottom.jpg", 1, true);
            u = new Texture(projectPath + "\\Textures\\skybox\\top.jpg", 2, true);
            l = new Texture(projectPath + "\\Textures\\skybox\\left.jpg", 3, true);
            r = new Texture(projectPath + "\\Textures\\skybox\\right.jpg", 4, true);
            f = new Texture(projectPath + "\\Textures\\skybox\\front.jpg", 5, true);
            b = new Texture(projectPath + "\\Textures\\skybox\\back.jpg", 6, true);
            groundTexture = new Texture(projectPath + "\\Textures\\ground.jpg", 7, true);
            shoot = new Texture(projectPath + "\\Textures\\gunshot.png", 8, true);
            cam = new Camera();
            cam.Reset(0, 34, 55, 0, 0, 0, 0, 1, 0);

            float[] square = {
                -1,0,1,   1,0,0,   0,0,
                1,0,1,    1,0,0,   1,0,
                -1,0,-1,  1,0,0,   0,1,

                1,0,1,    1,0,0,   1,0,
                -1,0,-1,  1,0,0,   0,1,
                1,0,-1,   1,0,0,   1,1
            };
            skyboxBuffer_ID = GPU.GenerateBuffer(square);

            up = MathHelper.MultiplyMatrices(new List<mat4> {
                glm.rotate(3.1412f, new vec3(1, 0, 0)),
                glm.translate(new mat4(1), new vec3(0,1,0))
            });

            down = glm.translate(new mat4(1), new vec3(0, -1, 0));
            // Dclare Matrices For transformation edges
            front = MathHelper.MultiplyMatrices(new List<mat4> {
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(1, 0, 0)),
                glm.translate(new mat4(1), new vec3(0,0,-1))
            });
            left = MathHelper.MultiplyMatrices(new List<mat4> {
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(1, 0, 0)),
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(0, 1, 0)),
                glm.translate(new mat4(1), new vec3(-1,0,0))
            });
            back = MathHelper.MultiplyMatrices(new List<mat4> {
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(1, 0, 0)),
                glm.rotate(3.1412f, new vec3(0, 1, 0)),
                glm.translate(new mat4(1), new vec3(0,0,1))
            });
            right = MathHelper.MultiplyMatrices(new List<mat4> {
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(1, 0, 0)),
                glm.rotate(-90.0f / 180.0f * 3.1412f, new vec3(0, 1, 0)),
                glm.translate(new mat4(1), new vec3(1,0,0))
            });

            ground = MathHelper.MultiplyMatrices(new List<mat4>
            {
                glm.scale(new mat4(1), new vec3(6000, 6000, 6000)),
                glm.translate(new mat4(1), new vec3(0, -45, 0))
            });
            building = new Model3D();
            building.LoadFile(projectPath + "\\ModelFiles\\static\\building", 1, "Building 02.obj");
            building.scalematrix = glm.scale(new mat4(1), new vec3(30, 30, 30));
            building.transmatrix = glm.translate(new mat4(1), new vec3(1, 1, 1));

            car = new Model3D();
            car.LoadFile(projectPath + "\\ModelFiles\\static\\car", 3, "dpv.obj");
            car.scalematrix = glm.scale(new mat4(1), new vec3(0.15f, 0.25f, 0.15f));
            car.transmatrix = glm.translate(new mat4(1), new vec3(-150, -45.0f, 0.5f));
            car.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));

            tree = new Model3D();
            tree.LoadFile(projectPath + "\\ModelFiles\\static\\tree", 2, "Tree.obj");
            tree.scalematrix = glm.scale(new mat4(1), new vec3(50, 50, 50));
            tree.transmatrix = glm.translate(new mat4(1), new vec3(-240, -45.0f, 0.5f));
            tree.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));

            Blade = new md2(projectPath + "\\ModelFiles\\animated\\md2\\blade\\Blade.md2");
            Blade.StartAnimation(animType.STAND);
            Blade.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            Blade.scaleMatrix = glm.scale(new mat4(1), new vec3(0.8f, 0.8f, 0.8f));
            Blade.TranslationMatrix = glm.translate(new mat4(1), new vec3(20, 19, -20));

            zombie = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            zombie.StartAnimation(animType_LOL.STAND);
            zombie.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            zombie.scaleMatrix = glm.scale(new mat4(1), new vec3(0.8f, 0.8f, 0.8f));

            
            spider = new Model3D();
            spider.LoadFile(projectPath + "\\ModelFiles\\static\\spider", 4, "spider.obj");
            spider.transmatrix = glm.translate(new mat4(1), new vec3(240, 0.0f, 0));
            spider.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(0, 1, 0));

            HandsWGun = new Model3D();
            HandsWGun.LoadFile(projectPath + "\\ModelFiles\\hands with gun", 8, "gun.obj");
            HandsWGun.scalematrix = glm.scale(new mat4(1), new vec3(0.2f, 0.2f, 0.2f));

            bulletModel = new Model3D();
            bulletModel.LoadFile(projectPath + "\\ModelFiles\\static\\bullet", 8, "bullet.obj");
            

            sh.UseShader();
            Gl.glClearColor(0, 0, 0.4f, 1);

        

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            modelID = Gl.glGetUniformLocation(sh.ID, "model");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            sh.UseShader();

            c = timer;
        }

        public void Draw()
        {
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);            
            sh.UseShader();
            // FOR DONW SQUARE
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, skyboxBuffer_ID);
            //FOR POSITION
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(0 * sizeof(float)));
            //FOR COLOR
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            // FOR TEXTURE
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            float[] viewmat = cam.GetViewMatrix().to_array();
            viewmat[12] = 0;
            viewmat[13] = 0;
            viewmat[14] = 0;
            viewmat[15] = 1;
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, viewmat);
            
            u.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, up.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            d.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, down.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            l.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, left.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            r.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, right.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            f.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, front.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            b.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, back.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());
            //ground
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            groundTexture.Bind();
            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, ground.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            playerPos = cam.GetCameraTarget();
            playerPos.y -= 2.8f;

            HandsWGun.transmatrix = glm.translate(new mat4(1), playerPos);
            HandsWGun.rotmatrix = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.rotate(-cam.mAngleY, new vec3(1, 0, 0)),
                glm.rotate(3.1412f + cam.mAngleX, new vec3(0, 1, 0))
            });


            spider.Draw(modelID);
            building.Draw(modelID);
            car.Draw(modelID);
            Blade.Draw(modelID);
            zombie.Draw(modelID);
            tree.Draw(modelID);
            HandsWGun.Draw(modelID);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, skyboxBuffer_ID);

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(0 * sizeof(float)));
            //FOR COLOR
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            // FOR TEXTURE
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

         
            shoot.Bind();
            vec3 shootpos = cam.GetCameraTarget();
            shootpos.y -= 1.5f;
            shootpos += cam.GetLookDirection() * 8;

            Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.rotate(cam.mAngleX, new vec3(0, 1, 0)),glm.rotate((float)c/10, new vec3(0, 0, 1)),
                glm.translate(new mat4(1),shootpos)
            }).to_array());
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            if (draw)
            {
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                c--;
                if (c < 0)
                {
                    c = timer;
                    draw = false;
                }
            }
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glDisable(Gl.GL_DEPTH_TEST);


            vec3 zombiePosition;
            zombiePosition.x = zombie.TranslationMatrix[3].x;
            zombiePosition.y = zombie.TranslationMatrix[3].y;
            zombiePosition.z = zombie.TranslationMatrix[3].z;

            vec3 zombieRotation = new vec3(0,-1,0);
            zombieRotation = zombie.rotationMatrix.to_mat3() * zombieRotation;

            vec3 camposition = cam.GetCameraPosition();

            vec3 zombieDirection = camposition - zombiePosition;
            zombieDirection = glm.normalize(zombieDirection);
            zombieDirection.y = 0;
            double distance = Math.Sqrt(Math.Pow(camposition.x - zombiePosition.x, 2) +
                                        Math.Pow(camposition.y - zombiePosition.y, 2) +
                                        Math.Pow(camposition.z - zombiePosition.z, 2));

            if (distance < Enemy_range_Run && distance > Enemy_range_Attack)
            {
                if (zombie.animSt.type != animType_LOL.RUN)
                {
                    zombie.StartAnimation(animType_LOL.RUN);
                }
                zombie.TranslationMatrix = glm.translate(zombie.TranslationMatrix, zombieDirection*zombieSpeed);
            }
            else if(distance < Enemy_range_Attack)
            {
                if (zombie.animSt.type != animType_LOL.ATTACK1)
                {
                    zombie.StartAnimation(animType_LOL.ATTACK1);
                }
            }
            else
            {
                if (zombie.animSt.type == animType_LOL.RUN)
                {
                    zombie.StartAnimation(animType_LOL.STAND);
                }
            }
            vec3 crossProduct = glm.cross(zombieRotation, zombieDirection);
            
            if (crossProduct.y != 0)
            {
                if(crossProduct.y > 0)
                    zombie.rotationMatrix = glm.rotate(zombie.rotationMatrix, (float)(5.0 / 180.0 *Math.PI), new vec3(0,0,1));
                else
                    zombie.rotationMatrix = glm.rotate(zombie.rotationMatrix, (float)(-5.0 / 180.0 * Math.PI), new vec3(0, 0, 1));
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(modelID);
                
            }
        }
        public bool draw = false;
        int timer = 10;
        int c;
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            zombie.UpdateAnimation();
            Blade.UpdateAnimation();

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update();
                if(bullets[i].shouldBeDestroyed())
                    bullets.RemoveAt(i);
            }
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
