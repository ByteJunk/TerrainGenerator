using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float meshHeightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;  // Vai servir para centrar verticalmente
        float topLeftZ = (height - 1) / 2f; // Vai servir para centrar horizontalmente

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                // A posição dos vértices na Mesh é baseado no canto superior esquerdo, para centrar
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * meshHeightMultiplier, topLeftZ - y);

                // O UV é posição em relação ao mapa, em percentagem (0 a 1) 
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // Calcular os triângulos ligados a cada vértice
                // Se estivermos na última linha ou coluna, não há mais triângulos
                if (x < width - 1 && y < height - 1)
                {
                    // Estamos no vértice a, vamos adicionar os dois triângulos abaixo.
                    // A ORDEM DE ROTACAO É IMPORTANTE!!!
                    // a - b
                    // | \ |
                    // c - d
                    int a = vertexIndex;
                    int b = vertexIndex + 1;
                    int c = vertexIndex + verticesPerLine; // +verticesPerLine = linha abaixo (depende do LoD)
                    int d = vertexIndex + verticesPerLine + 1;

                    // Triângulo definido pelos vértices: a-d-c
                    meshData.AddTriangles(a, d, c);
                    // Triângulo definido pelos véritices: d-a-b
                    meshData.AddTriangles(d, a, b);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    public void AddTriangles(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }


    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh()
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}