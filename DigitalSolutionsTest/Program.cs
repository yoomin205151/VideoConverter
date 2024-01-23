using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

class Program
{
    static void Main()
    {
        // Crear una imagen en blanco
        Mat image = new Mat(200, 400, DepthType.Cv8U, 3);

        // Dibujar un rectángulo en la imagen
        CvInvoke.Rectangle(image, new System.Drawing.Rectangle(10, 10, 180, 80), new MCvScalar(255, 0, 0), 2);

        // Mostrar la imagen en la consola
        Console.WriteLine("Presiona cualquier tecla para salir.");
        CvInvoke.Imshow("Imagen de prueba", image);
        CvInvoke.WaitKey(0);
    }
}
