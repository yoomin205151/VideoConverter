using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace DigitslSolutionsTest
{
    public partial class Form1 : Form
    {
        private VideoCapture _capture;
        private bool _isCapturing;
        private string _rutaBaseGuardarFrames;
        public Form1()
        {
            _capture = new VideoCapture();
            _rutaBaseGuardarFrames = @"C:\Imagenes"; 
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de video|*.mp4;*.avi;*.mov|Todos los archivos|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string nombreVideo = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                _rutaBaseGuardarFrames = Path.Combine(_rutaBaseGuardarFrames, nombreVideo);

                // Crear la carpeta para el video
                if (!Directory.Exists(_rutaBaseGuardarFrames))
                {
                    Directory.CreateDirectory(_rutaBaseGuardarFrames);
                }

                _capture = new VideoCapture(openFileDialog.FileName);
                _isCapturing = true;

                // Iniciar la reproducción del video
                Application.Idle += CapturarFrame;
            }
        }

        private void btn_salir_Click(object sender, EventArgs e)
        {
            // Detener la captura y liberar recursos
            _isCapturing = false;
            Application.Idle -= CapturarFrame;
            _capture.Dispose();


            // Cerrar la aplicación
            Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void CapturarFrame(object sender, EventArgs e)
        {
            Mat frame = _capture.QueryFrame();

            if (frame != null)
            {
                // Analizar la cara y detectar diferencias faciales
                AnalizarCara(frame);

                // Mostrar el frame en el PictureBox
                pictureBox1.Image = frame.ToImage<Bgr, byte>().ToBitmap();

                // Guardar el frame como una imagen
                GuardarFrameComoImagen(frame);
            }
            else
            {
                // Si el video termina, detener la captura
                _isCapturing = false;
                Application.Idle -= CapturarFrame;
                MessageBox.Show("El video ha terminado de cargarse.", "Fin del video", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void GuardarFrameComoImagen(Mat frame)
        {
            // Asegura de que la carpeta de destino exista
            if (!Directory.Exists(_rutaBaseGuardarFrames))
            {
                Directory.CreateDirectory(_rutaBaseGuardarFrames);
            }

            // Crea un nombre único para la imagen basado en la hora actual
            string nombreImagen = $"frame_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            // Guardar el frame como una imagen en la carpeta de destino
            string rutaCompleta = Path.Combine(_rutaBaseGuardarFrames, nombreImagen);
            frame.ToImage<Bgr, byte>().ToBitmap().Save(rutaCompleta);
        }

        private void AnalizarCara(Mat frame)
        {

            // Convertir el frame a escala de grises para la detección de rostros
            Mat grayFrame = new Mat();
            CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

            // Crear clasificadores de cascada para la detección facial, ojos, boca y nariz
            CascadeClassifier faceCascade = new CascadeClassifier("C:\\Users\\yoomi\\source\\repos\\DigitalSolutionsTest\\DigitslSolutionsTest\\Haarcascade\\haarcascade_frontalface_default.xml");
            CascadeClassifier eyesCascade = new CascadeClassifier("C:\\Users\\yoomi\\source\\repos\\DigitalSolutionsTest\\DigitslSolutionsTest\\Haarcascade\\haarcascade_eye.xml");


            // Detectar rostros en el frame
            Rectangle[] faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 5, Size.Empty);

            foreach (var face in faces)
            {
                // Dibujar un rectángulo alrededor del rostro
                CvInvoke.Rectangle(frame, face, new MCvScalar(0, 255, 0), 2);

                // Definir la región de interés (ROI) para los ojos, boca y nariz
                Rectangle roiFace = new Rectangle(face.X, face.Y, face.Width, face.Height);
                Mat regionOfInterest = new Mat(grayFrame, roiFace);

                // Detectar ojos en la región de interés
                Rectangle[] eyes = eyesCascade.DetectMultiScale(regionOfInterest, 1.1, 3, new Size(60, 0));
                foreach (var eye in eyes)
                {

                    // Filtrar regiones de interés basadas en posición, tamaño y relación de aspecto
                    double aspectRatio = (double)eye.Width / eye.Height;
                    if (eye.X > regionOfInterest.Width * 0.1 && eye.Y > regionOfInterest.Height * 0.1 &&
                        eye.X + eye.Width < regionOfInterest.Width * 0.9 && eye.Y + eye.Height < regionOfInterest.Height * 0.9 &&
                        aspectRatio > 0.2 && aspectRatio < 2.0)
                    {

                        // Dibujar un rectángulo alrededor de cada ojo
                        Rectangle eyeRect = new Rectangle(face.X + eye.X, face.Y + eye.Y, eye.Width, eye.Height);
                        CvInvoke.Rectangle(frame, eyeRect, new MCvScalar(255, 0, 0), 2);

                    }
                }
            }

            // Mostrar el frame modificado en el PictureBox
            pictureBox1.Image = frame.ToImage<Bgr, byte>().ToBitmap();

        }


    }
}
