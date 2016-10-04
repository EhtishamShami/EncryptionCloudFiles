using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace EncryptionDecryption
{
    public partial class Form1 : Form
    {
        string original;
        public Form1()
        {
            InitializeComponent();
            original = "";
        }
     
        private void button1_Click(object sender, EventArgs e)
        {
            /////Credit goes to msdn.com
            try {
                
                using (TripleDESCryptoServiceProvider tripledes=new TripleDESCryptoServiceProvider())
                {
                    ////I am Encrpyting the string original to array of bytes
                    byte[] encrypted = EncryptStringToBytes(original, tripledes.Key, tripledes.IV);
                    ////Now its time to roll back the encryption and decryption
                    File.WriteAllBytes("encrypted",encrypted);
                    Byte[] key = tripledes.Key;
                    File.WriteAllBytes("key",key);
                    Byte[] tripledesIV = tripledes.IV;
                    File.WriteAllBytes("tripledesIV", tripledesIV);
                    encrypted = File.ReadAllBytes("encrypted");
                    string roundtrip = DecryptBytesToString(encrypted, key, tripledesIV);
                    MessageBox.Show("Original "+original+" Round trip "+roundtrip);
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.StackTrace.ToString());
                
            }
        }
        static byte[] EncryptStringToBytes(string plaintext,byte[] key,byte[] IV)
        {

            //////Check for valid arguments
            if (plaintext == null|| plaintext.Length<=0)
            {
                throw new ArgumentNullException("plaintext");
            }
            if (key == null||key.Length<=0)
            {
                throw new ArgumentNullException("KEY");
            }
            if (IV==null||IV.Length<=0)
            {
                throw new ArgumentNullException("KEY");
            }

            byte[] encrypted;
            // Create an TripleDESCryptoServiceProvider object
            // with the specified key and IV.
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.Key = key;
                tdsAlg.IV = IV;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encrypter = tdsAlg.CreateEncryptor(tdsAlg.Key,tdsAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plaintext);
                        }
                       encrypted = msEncrypt.ToArray();                                         
                    }
                }
            }

        
        // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        static string DecryptBytesToString(byte[] ciphertext,byte[] key,byte[] IV)
        {
            // Check arguments.
            if (ciphertext == null || ciphertext.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an TripleDESCryptoServiceProvider object
            // with the specified key and IV.
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.Key = key;
                tdsAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = tdsAlg.CreateDecryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;   ////T select multiple file set it to true i am seleccting it to false
           
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    if ((myStream=openFileDialog1.OpenFile())!=null)
                    {
                        using (StreamReader reader=new StreamReader(myStream))
                        {
                            // Read the first line from the file and write it the textbox.
                            original = reader.ReadLine();
                        }

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }

            }


        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (TripleDESCryptoServiceProvider tripledes = new TripleDESCryptoServiceProvider())
            {
                Byte[] encrypted = File.ReadAllBytes("encrypted");
                Byte[] key = File.ReadAllBytes("key");
                Byte[] tripledesIV = File.ReadAllBytes("tripledesIV");
                string roundtrip = DecryptBytesToString(encrypted, key, tripledesIV);
                MessageBox.Show("Original " + original + " Round trip " + roundtrip);
            }
        }
    }
}
