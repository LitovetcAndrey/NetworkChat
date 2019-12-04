using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<string> mesages = new List<string>();
        TcpClient tcpClient = null;
        const int PORT = 55_555;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", PORT);

                Listen(tcpClient);



            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
               // tcpClient.Close();
            }

        }

        private  async void Listen(TcpClient client)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] bytesRead = new byte[256];

                        NetworkStream stream = client.GetStream();

                        int length = stream.Read(bytesRead, 0, bytesRead.Length);
                        string message = Encoding.ASCII.GetString(bytesRead, 0, length);

                        mesages.Add(message);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            lbMessages.Items.Add(String.Format("{0}  {1}", DateTime.Now.Date, message));
                        }));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);

                    }

                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] buff = Encoding.ASCII.GetBytes(tbMessage.Text);
            tcpClient.Client.Send(buff);

            lbMessages.Items.Add(tbMessage.Text);
            tbMessage.Text = "";

        }
    }


}

