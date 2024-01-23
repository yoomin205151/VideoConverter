using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
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

        private void btn_add_Click(object sender, EventArgs e)
        {
            // Evento de clic del botón para agregar un archivo de video

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de video|*.mp4;*.avi;*.mov|Todos los archivos|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string nombreVideo = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                _rutaBaseGuardarFrames = Path.Combine(_rutaBaseGuardarFrames, nombreVideo);

                if (!Directory.Exists(_rutaBaseGuardarFrames))
                {
                    Directory.CreateDirectory(_rutaBaseGuardarFrames);
                }

                _capture = new VideoCapture(openFileDialog.FileName);
                _isCapturing = true;
         
                Application.Idle += CapturarFrame;
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            // Evento de clic del botón para salir de la aplicación

            _isCapturing = false;
            Application.Idle -= CapturarFrame;
            _capture.Dispose();
            Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void CapturarFrame(object sender, EventArgs e)
        {

            // Manejador de eventos para capturar frames del video

            Mat frame = _capture.QueryFrame();

            if (frame != null)
            {
                AnalyzeFace(frame);

                pictureBox1.Image = frame.ToImage<Bgr, byte>().ToBitmap();

                SaveFrameAsImage(frame);

            }
            else
            {               
                _isCapturing = false;
                Application.Idle -= CapturarFrame;
                MessageBox.Show("El video ha terminado de cargarse.", "Fin del video", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void SaveFrameAsImage(Mat frame)
        {

            // Guardar el frame actual como una imagen

            if (!Directory.Exists(_rutaBaseGuardarFrames))
            {
                Directory.CreateDirectory(_rutaBaseGuardarFrames);
            }

            
            string nombreImagen = $"frame_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            
            string rutaCompleta = Path.Combine(_rutaBaseGuardarFrames, nombreImagen);
            frame.ToImage<Bgr, byte>().ToBitmap().Save(rutaCompleta);
        }

        private void AnalyzeFace(Mat frame)
        {

            // Analizar rostros, ojos y dibujar rectángulos alrededor de ellos

            Mat grayFrame = new Mat();
            CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

            
            CascadeClassifier faceCascade = new CascadeClassifier("C:\\Users\\yoomi\\source\\repos\\DigitalSolutionsTest\\DigitslSolutionsTest\\Haarcascade\\haarcascade_frontalface_default.xml");
            CascadeClassifier eyesCascade = new CascadeClassifier("C:\\Users\\yoomi\\source\\repos\\DigitalSolutionsTest\\DigitslSolutionsTest\\Haarcascade\\haarcascade_eye.xml");

 
            Rectangle[] faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 5, Size.Empty);

            foreach (var face in faces)
            {
                
                CvInvoke.Rectangle(frame, face, new MCvScalar(0, 255, 0), 2);

                
                Rectangle roiFace = new Rectangle(face.X, face.Y, face.Width, face.Height);
                Mat regionOfInterest = new Mat(grayFrame, roiFace);

                
                Rectangle[] eyes = eyesCascade.DetectMultiScale(regionOfInterest, 1.1, 3, new Size(60, 0));
                foreach (var eye in eyes)
                {
          
                    double aspectRatio = (double)eye.Width / eye.Height;
                    if (eye.X > regionOfInterest.Width * 0.1 && eye.Y > regionOfInterest.Height * 0.1 &&
                        eye.X + eye.Width < regionOfInterest.Width * 0.9 && eye.Y + eye.Height < regionOfInterest.Height * 0.9 &&
                        aspectRatio > 0.2 && aspectRatio < 2.0)
                    {

                        Rectangle eyeRect = new Rectangle(face.X + eye.X, face.Y + eye.Y, eye.Width, eye.Height);
                        CvInvoke.Rectangle(frame, eyeRect, new MCvScalar(255, 0, 0), 2);

                    }
                }
            }

            pictureBox1.Image = frame.ToImage<Bgr, byte>().ToBitmap();

        }


    }
}
