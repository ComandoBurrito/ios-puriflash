using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;



/// <summary>
/// IMPORTANT STUFF
/// UTType List : https://developer.apple.com/library/archive/documentation/Miscellaneous/Reference/UTIRef/Articles/System-DeclaredUniformTypeIdentifiers.html
/// </summary>
namespace Json_BLE
{
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //Variables
        string txtSourceCode = null;
        bool switchButton = true;

        private void EnumerateButton_Clicked(object sender, EventArgs args)
        {
            switchButton = !switchButton;
            if (switchButton == false)
            {
                //StartBleDeviceWatcher();
                EnumerateButton.Text = "Stop enumerating";
                LoadIndicator.IsRunning = true;
                LoadIndicator.IsVisible = true;
                LoadIndicator.IsEnabled = true;
                //rootPage.NotifyUser($"Device watcher started.", NotifyType.StatusMessage);
            }
            else
            {
                //StopBleDeviceWatcher();
                EnumerateButton.Text = "Start enumerating";
                LoadIndicator.IsRunning = false;
                LoadIndicator.IsVisible = false;
                LoadIndicator.IsEnabled = false;
                //rootPage.NotifyUser($"Device watcher stopped.", NotifyType.StatusMessage);
            }
        }

        private async void PickFile_Clicked(object sender, EventArgs args)
        {
            string[] fileTypes = null;

            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] { "files/json" };
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                fileTypes = new string[] { "public.json" }; // same as iOS constant UTType.Image
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] { ".json" };
            }

            if (Device.RuntimePlatform == Device.WPF)
            {
                fileTypes = new string[] { "JSON files (*.json)|*.json" };
            }

            await PickAndShowFile(fileTypes); //File Picker Menu
        }

        private async void PickImage_Clicked(object sender, EventArgs args)
        {
            string[] fileTypes = null;

            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] { "image/png", "image/jpeg" };
            }
            if (Device.RuntimePlatform == Device.iOS)
            {
                fileTypes = new string[] { "public.image" }; // same as iOS constant UTType.Image
            }
            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] { ".jpg", ".png" };
            }
            if (Device.RuntimePlatform == Device.WPF)
            {
                fileTypes = new string[] { "JPEG files (*.jpg)|*.jpg", "PNG files (*.png)|*.png" };
            }

            await PickAndShowFile(fileTypes);
        }

        private async void PickZip_Clicked(object sender, EventArgs args)
        {
            string[] fileTypes = null;

            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] { "application/zip" };
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                fileTypes = new string[] { "com.pkware.zip-archive" }; //Watch the 'UTType List' for more information
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] { ".zip" };
            }

            if (Device.RuntimePlatform == Device.WPF)
            {
                fileTypes = new string[] { "ZIP files (*.zip)|*.zip" };
            }

            await PickAndShowFile(fileTypes); //File Picker Menu
        }

/*========================================================================================================*/
        
        private async Task PickAndShowFile(string[] fileTypes)
        {
            try
            {
                var pickedFile = await CrossFilePicker.Current.PickFile(fileTypes); //Picked file

                if (pickedFile != null)
                {

                    FileNameLabel.Text = pickedFile.FileName;  //File name
                    FilePathLabel.Text = pickedFile.FilePath;  //Path
                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Get the simulator documents folder
                    var filepath = Path.Combine(documents, pickedFile.FileName); //alt : (documents, "Write2.txt");

                    if (pickedFile.FileName.EndsWith(".json")) //If you pick a .json file
                    {
                        FileNameLabel.IsVisible = false;
                        FilePathLabel.IsVisible = false;
                        FileTextLabel.IsVisible = false;
                        FileJsonLabel.IsVisible = true;
                        FileImagePreview.IsVisible = false;
                        FileBase64Label.IsVisible = false;
                        ShowMsgLabel.Text = "String of the JSON file";


                        StreamReader strFile = new StreamReader(pickedFile.GetStream()); //The whole content of the stream in a STRING
                        FileTextLabel.Text = strFile.ReadToEnd(); //If there's something in the "Label" remember that it shows by itself
                        strFile.Close();   //Unless you delete it from the xaml

                        splitProcess(FileTextLabel.Text);

                        /*=========== SIMULACION DE RECEPCION DEL LADO DEL SERVER ===========*/
                        FileJsonLabel.Text = fileString; //!!!!!!!!!!! <<<<=============
                        File.WriteAllText(filepath, fileString); //Crea un archivo : System.IO (string path, string contents)
                        /*=========== SIMULACION DE RECEPCION DEL LADO DEL SERVER ===========*/
                    }
                    else if (pickedFile.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase) //If you pick an img (.jpeg or .png)
                        || pickedFile.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        FileNameLabel.IsVisible = false;
                        FilePathLabel.IsVisible = false;
                        FileTextLabel.IsVisible = false;
                        FileJsonLabel.IsVisible = false;
                        FileBase64Label.IsVisible = true;
                        ShowMsgLabel.Text = "Base64 of the JPEG file";

                        FileImagePreview.Source = ImageSource.FromStream(() => pickedFile.GetStream()); //var Image type
                        txtSourceCode = FileToBase64(FilePathLabel.Text); //Transforma la img de acuerdo su ruta
                        
                        splitProcess(txtSourceCode);

                        /*=========== SIMULACION DE RECEPCION DEL LADO DEL SERVER ===========*/
                        FileBase64Label.Text = fileString;
                        byte[] data = Convert.FromBase64String(FileBase64Label.Text); //Convierte de Base64 a array (byte[])
                        File.WriteAllBytes(filepath, data); //Crea un archivo : System.IO (string path, string contents)
                        FileImagePreview.IsVisible = true; //At this point, the picked image is now visible
                        /*=========== SIMULACION DE RECEPCION DEL LADO DEL SERVER ===========*/
                    }
                    else if (pickedFile.FileName.EndsWith("zip")) //If you pick a .zip file
                    {
                        FileNameLabel.IsVisible = false;
                        FilePathLabel.IsVisible = false;
                        FileTextLabel.IsVisible = false;
                        FileJsonLabel.IsVisible = false;
                        FileImagePreview.IsVisible = false;
                        FileBase64Label.IsVisible = true;
                        ShowMsgLabel.Text = "Base64 of the ZIP file";
                        txtSourceCode = FileToBase64(FilePathLabel.Text); //Transforma el zip de acuerdo su ruta

                        splitProcess(txtSourceCode);

                        /*=========== SIMULACION DE RECEPCION DEL LADO DEL SERVER ===========*/
                        FileBase64Label.Text = fileString;
                        byte[] data = Convert.FromBase64String(FileBase64Label.Text); //Convierte de Base64 a array (byte[])
                        File.WriteAllBytes(filepath, data); //Crea un archivo : System.IO (string path, string contents)
                        /*=========== SIMULACION DE RECEPCION DEL LADO DEL SERVER ===========*/
                    }
                    else
                    {
                        FileImagePreview.IsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                FileNameLabel.Text = ex.ToString();
                FilePathLabel.Text = string.Empty;
                FileImagePreview.IsVisible = false;
            }
        }

        /*========================================================================================================*/

        private void splitProcess(string str512)
        {
            int chunkSize = 512;
            int stringLength = str512.Length;
            receiveStr("start");
            for (int i = 0; i < stringLength; i += chunkSize)
            {
                if (i + chunkSize > stringLength)
                {
                    chunkSize = stringLength - i;
                }
                receiveStr(str512.Substring(i, chunkSize));
            }
            receiveStr("end");
        }

        public string FileToBase64(string path)
        {
            // provide read access to the file
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            // Create a byte array of file stream length
            byte[] ImageData = new byte[fs.Length];
            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
            //Close the File Stream
            fs.Close();
            string base64String = Convert.ToBase64String(ImageData);
            return base64String;
        }

        /// <summary>
        /// Gatt server string Reception Process [iOS OFFLINE MODE]
        /// ====== Only reconstruction so far ======
        /// </summary>
        /// <param name="recepStr"></param>
        //14h17 : Hace falta agregar un reseteo al "strConstruct.Append" al final de la funcion
        //15h15 : Fixed, utilizando .Clear();
        StringBuilder strConstruct = new System.Text.StringBuilder(); //Probablemente aqui se deba de poner 
        string fileString; //Resultado de concatenarlo todo
        bool currentState; //Booleano que me permititra seguir concatenando
        private void receiveStr(string recepStr)
        {
            if (recepStr == "start")
            {
                currentState = true;
            }
            else if (recepStr == "end")
            {
                currentState = false;
            }

            if (currentState == true) //Proceso normal
            {
                if (recepStr != "start")
                {
                    strConstruct.Append(recepStr); //Inserta un pedazo mas al string buffer
                }
            }
            else if (currentState == false)
            {
                fileString = strConstruct.ToString(); //Se iguala el string final al string buffer
                strConstruct.Clear(); //Se vacia el string buffer. == fileString recepta toda el contenido al final ==
            }
        }
    }
}