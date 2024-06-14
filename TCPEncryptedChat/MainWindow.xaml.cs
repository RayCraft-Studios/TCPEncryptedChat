using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
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

namespace TCPEncryptedChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Public Variablen Initialisieren:
        //Deine IP
        public string yIP;
        //Ziel IP
        public string destIP;
        //Port
        public int port = 5503;
        //Schlüssel für Verschlüsselung
        public string yKey;
        public string Okey;
        bool keepAlive;

        //Objekte erstellen
        private Crypter crypter = new Crypter();
        private UdpClient udpClient;
        private Thread listenThread;

        public MainWindow()
        {
            InitializeComponent();
            YIP.Text = GetYourIP();
            //UDP Client mit Port erstellen
            udpClient = new UdpClient(port);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //Testen ob alle Felder Voll!!
            if (!string.IsNullOrEmpty(ConnectToIP.Text) 
                && !string.IsNullOrEmpty(YKey.Text) 
                && !string.IsNullOrEmpty(OKey.Text))
            {
                //Falls alles ausgefüllt wurde!!
                Error.Text = "";
                //Chat Page erscheinen lassen
                ChatSite.Visibility = Visibility.Visible;
                //Variablen setzen
                keepAlive = true;   
                destIP = ConnectToIP.Text;
                yKey = YKey.Text;
                Okey = OKey.Text;
                //Verbindung starten
                StartListening();
            }
            else
            {
                //Fehlermeldung anzeigen
                Error.Text = "Bitte alle Felder Ausfüllen";
            }
        }


        //alles für die Connection
        public string GetYourIP()
        {
            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            yIP = localIP;
            return localIP;
        }

        //Sende Button
        private void SendM_Click(object sender, RoutedEventArgs e)
        {
            //Deinen eingegebenen Text als String speichern
            string message = YSend.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                //Falls der String nicht Leer ist wird er der SendMessage Methode übergeben
                SendMessage(message, destIP);
                //Box wird geleert
                YSend.Text = string.Empty;
            }
        }

        //Zuhören Starten
        private void StartListening()
        {
            //ListenForMessages als neuen Thread Initialisieren
            listenThread = new Thread(new ThreadStart(ListenForMessages));
            //ListenForMessage als hintergrund starten
            listenThread.IsBackground = true;
            //Thread Starten
            listenThread.Start();
        }

        //Auf Nachrichten hören
        public void ListenForMessages()
        {
            try
            {
                while (keepAlive)
                {
                    //Neues Objekt IPEndPoint mit der Ziel IP und Port setzen
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(destIP), port);
                    //Erhaltene Daten als Bytes speichern
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    //Erhaltene Bytes als UTF8 encodeten String Speichern
                    string message = Encoding.UTF8.GetString(data);
                    //String Key mit der decodeKey und deinen Schlüssel Entschlüsseln
                    string encoded = crypter.decodeKey(message, yKey);
                    //Erhaltene Nachricht anzeigen lassen
                    AddMessage($"Partner {remoteEndPoint}: {encoded}");
                }
            }
            catch (Exception ex)
            {
                //Error als Message Box anzeigen!!
                MessageBox.Show("Error occurred: " + ex.Message);
            }
        }

        //Nachrichten senden
        private void SendMessage(string message, string destinationIP)
        {
            try
            {
                //Neuer UDPClient initialisieren
                UdpClient sender = new UdpClient();
                //Neuer IPEndpoint mit ZielIP und Port Setzen
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(destinationIP), port);
                //Zu versendender Text mit der encodeKey Methode mit dem Schlüssel des Gegenüber verschlüsseln
                string toSend = crypter.encodeKey(message, Okey);
                //String zu Bytearray speichern
                byte[] data = Encoding.UTF8.GetBytes(toSend);
                //Daten senden
                sender.Send(data, data.Length, endPoint);
                //Sender Schliessem
                sender.Close();
                //Eignene Nachricht als Klartext im Chat anzeigen
                AddMessage($"Gesendet an {destinationIP}: {message}");
            }
            catch (Exception ex)
            {
                //Error als Message Box anzeigen
                MessageBox.Show("Error occurred: " + ex.Message);
            }
        }

        //Nachricht hinzufügen
        private void AddMessage(string message)
        {
            //Nachricht an Chatbox anhängen
            Dispatcher.Invoke(() =>
            {
                ChatBox.AppendText(message + "\n");
            });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            keepAlive = false;
            ChatBox.Text = "";
            YSend.Text = "";
            ChatSite.Visibility = Visibility.Hidden;
        }
    }
}
