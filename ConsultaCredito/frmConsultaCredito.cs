using BibliotecaBanco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ConsultaCredito
{
    public partial class frmConsultaCredito : Form
    {

        private FileStream entrada; // mantiene la conexión al archivo 
        private StreamReader ArchivoReader; //Lee los componentes de un archivo 
        //Nombre del archivo que almacena 
        private string NombreArchivo; 
        public frmConsultaCredito()
        {
            InitializeComponent();
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectorDeArchivo = new OpenFileDialog();
            DialogResult result = selectorDeArchivo.ShowDialog();

            if (result == DialogResult.Cancel)
                return; 
            NombreArchivo = selectorDeArchivo.FileName; 
            if(NombreArchivo == null || NombreArchivo== "")
            {
                MessageBox.Show("nombre del archivo invalido","Error", 
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                entrada= new FileStream(NombreArchivo, FileMode.Open,
                    FileAccess.Read);
                ArchivoReader = new StreamReader(entrada);
                btnAbrir.Enabled = false;
               btnCero.Enabled = true;
                btnCrredito.Enabled = true;
                btnDebito.Enabled = true;   
            }
        }

        //Se invoca cuando el usuario hace click en saldos con creditos 
        //saldos con debito o saldo con cero 
        private void ObtenerSaldos_Click(object sender, EventArgs e)
        {
            //convierte el emisor explicitamente a un objeto de tipo button 
            Button EmisorButton = (Button)sender;

            //obtiene el texto del boton en el que se hizo click; y que almacena el tipo de la cuenta 
            string  tipoCuenta = EmisorButton.Text;
            //Lee y muestra la información del archivo

            try
            {
                //regresa al principio del archivo 
                entrada.Seek(0, SeekOrigin.Begin);
                txtMostrar.Text = "Las cuentas  son:\r\n";

                //recorrer el archivo hasta llegar a su fin 
                while (true)
                {
                    string[] camposEntrada;//Almacena piezas de datos individuales 
                    Registro registro;//almacena cada registro a medida que se lee el archivo
                    decimal saldo; //almacena el saldo de cada registro 

                    //obtiene el siguiente registro disponible en el archivo 
                    string registroEntrada = ArchivoReader.ReadLine();
                    //Cuando esta al final del archivo sale del  metodo
                    if (registroEntrada == null)
                        return;

                    camposEntrada = registroEntrada.Split(',');//Analiza la entrada 

                    //crrea el regitor a partir de entrada 

                    registro = new Registro(Convert.ToInt32(camposEntrada[0]), camposEntrada[1], camposEntrada[2], Convert.ToDecimal(camposEntrada[3]));

                    //almacena l ultimo registro en saldo 
                    saldo = registro.Saldo;

                    //determina si va  amostrar el saldo o no 

                    if (DebeMostrar(saldo, tipoCuenta))
                    {
                        //Muestra el registro 
                        string salida = registro.Cuenta + "\t" +
                            registro.PrimerNombre + "\t" + registro.ApellidoPaterno + "\t";

                        //muestra el saldo con el dormato monetario 
                        salida += string.Format("{0:F}", saldo + "\r\n");
                        txtMostrar.Text+= salida;
                    }



                }
            }
            catch (IOException)
            {

                MessageBox.Show("NO se puede leer el archivo", "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool DebeMostrar(decimal saldo, string tipoCuenta)
        {
            if(saldo> 0)
            {
                //muestra los saldos con credito 
                if (tipoCuenta == "Saldo con credito")
                    return true; 
            }
            else if(saldo< 0)
            {
                //Muestra los saldos con debido
                if (tipoCuenta == "Saldos con Debito")
                    return true;
            }
            else//saldo == 0
            {
                //Muestra los saldos en cero

                if (tipoCuenta == "Saldos en Cero")
                    return true;
            }
            return false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if(entrada!= null)
            {
                try
                {
                    entrada.Close();    
                    ArchivoReader.Close();
                }
                catch(IOException)
                {
                    MessageBox.Show("NO se puede cerrar el archivo", "Error ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Application.Exit(); 
        }
    }
}
