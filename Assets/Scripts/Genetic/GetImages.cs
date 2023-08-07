using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public class GetImages : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] CameraOutput cameraOutputs;

    private int index = 0;
    public GameObject lastObject;
    private byte[][][] images_to_be_sent; // n x 2 x sizeimage
    private string model_images_path = @"C:\Users\Yan\Desktop\Robotic hand\Scripts\Genetic Server\images";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public byte[][] GetImagesFromCameras()
    {
        RenderTexture[] images = cameraOutputs.GetCamerasOutputs();
        byte[][] result = new byte[images.Length][];
        for (int i = 0; i < images.Length; i++)
        {
            result[i] = GetRenderTexturePixels(images[i]);
        }
        return result;
    }
    public string[][] RecordImages()
    {
        if(lastObject != null)
            DestroyImmediate(lastObject);
        images_to_be_sent = new byte[objects.Length][][];
        for (int i = 0; i < objects.Length; i++)
        {
            if (i == 0)
            {
                spawnObject(objects[0]);
            }
            else
                SpawnNextObject();

            RenderTexture[] images = cameraOutputs.GetCamerasOutputs();
            images_to_be_sent[i] = new byte[images.Length][];
            // convert re
            for (int j = 0; j < images.Length; j++)
            {
                // convert texture to byte array to send
                byte[] colourArray = GetRenderTexturePixels(images[j]);
                // save it in an array to be sent later to reduce latency
                images_to_be_sent[i][j] = colourArray;
            }
        }
        // save images
        string[][] paths = saveImagesOnDisk(images_to_be_sent, model_images_path);
        return paths;
    }
    public string[][] saveImagesOnDisk(byte[][][] images, string path)
    {
        string[][] paths = new string[images.Length][];
        for(int i=0; i< images.Length; i++)
        {
            paths[i] = new string[images[i].Length];
            for(int j=0; j < images[i].Length; j++)
            {
                paths[i][j] = path + @"\object_" + i.ToString()
                    + "_perspective_" + j.ToString() + ".png";
                if (File.Exists(paths[i][j]))
                    File.Delete(paths[i][j]);
                File.WriteAllBytes(paths[i][j], images[i][j]);
            }
        }
        return paths;
    }

    public void saveGenerationInfo(byte[][][] images, string path, int gen_num)
    {
        string dir_path = path + "/generation_" + gen_num.ToString();
        if (!Directory.Exists(dir_path))
            Directory.CreateDirectory(dir_path);
        saveImagesOnDisk(images, dir_path);
    }

    public void saveImageOnDisk(string path_name, int dna_index)
    {
        RenderTexture[] images = cameraOutputs.GetCamerasOutputs();
        for(int i=0; i< images.Length; i++)
        {
            // convert texture to byte array to send
            byte[] colourArray = GetRenderTexturePixels(images[i]);
            string name = path_name + "_perspective_" + i.ToString() + ".png";
            if (File.Exists(name))
                File.Delete(name);
            File.WriteAllBytes(name, colourArray);
        }
    }
    public byte[] GetRenderTexturePixels(RenderTexture tex)
    {
        RenderTexture.active = tex;
        Texture2D tempTex = new Texture2D(tex.width, tex.height);
        tempTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tempTex.Apply();
        return tempTex.EncodeToPNG();
    }
  

    public void SpawnNextObject()
    {
        DestroyImmediate(lastObject);
        index = (index + 1) % objects.Length;
        lastObject = spawnObject(objects[index]);
    }
    public void SpawnNextObject(int obj_index)
    {
        DestroyImmediate(lastObject);
        lastObject = spawnObject(objects[obj_index]);
    }
    protected virtual GameObject spawnObject(GameObject prefabToSpawn)
    {
        //Debug.Log("Spawning a new object");

        // Step 1: spawn the new object.
        GameObject newObject = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        lastObject = newObject;
        
        // lastObject.transform.localScale = new Vector3(2, 2, 2);
        return newObject;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
