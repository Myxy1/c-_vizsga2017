using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace vizsga2017_Merenyi_Miklos_14S
{
    public partial class Form4 : Form
    {
        MySqlConnection kapcsolat;
        private DataTable eredmenytabla;
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            try
            {
                string kapcsolatString = "SERVER=localhost;" + "DATABASE=befektetes;" + "UID=root;" + "PASSWORD=;";
                kapcsolat = new MySqlConnection(kapcsolatString);
                kapcsolat.Open();
                TanacsadoFelvetele();
                UgyfelnevFelvetele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Probléma az adatbázis megnyitásakor! " + ex.Message);
                Close();
            }
        }

        private void TanacsadoFelvetele()
        {
            try
            {                 //Tanácsadó előállítása                 
                String parancsStr = "SELECT " +
                            "tanacsado.nev as tanacsado " +
                            "FROM " +
                            "szakterulet, tanacsado, talalkozo, ugyfel " +
                            "WHERE " +
                            "szakterulet.szakterulet_id = tanacsado.szakterulet_id AND " +
                            "tanacsado.tanacsado_id = talalkozo.tanacsado_id AND " +
                            "talalkozo.ugyfel_id = ugyfel.ugyfel_id " +
                            "GROUP BY tanacsado";
                MySqlCommand parancs = new MySqlCommand(parancsStr, kapcsolat);
                eredmenytabla = new DataTable();
                new MySqlDataAdapter(parancs).Fill(eredmenytabla);
                //A Tanácsadók betöltése a combobox-ba      
                MySqlDataReader reader = parancs.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["tanacsado"]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a nevek feltöltésekor! " + ex.Message);
            }
        }

        private void UgyfelnevFelvetele()
        {
            try
            {                 //Ügyfélnév előállítása                 
                String parancsStr = "SELECT " +
                            "ugyfel.nev as ugyfel " +
                            "FROM " +
                            "szakterulet, tanacsado, talalkozo, ugyfel " +
                            "WHERE " +
                            "szakterulet.szakterulet_id = tanacsado.szakterulet_id AND " +
                            "tanacsado.tanacsado_id = talalkozo.tanacsado_id AND " +
                            "talalkozo.ugyfel_id = ugyfel.ugyfel_id " +
                            "GROUP BY ugyfel";
                MySqlCommand parancs = new MySqlCommand(parancsStr, kapcsolat);
                eredmenytabla = new DataTable();
                new MySqlDataAdapter(parancs).Fill(eredmenytabla);
                //A Ügyfélnevek betöltése a combobox-ba      
                MySqlDataReader reader = parancs.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["ugyfel"]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a nevek feltöltésekor! " + ex.Message);
            }
        }

        private void UjAdatFelvetele()
        {
            String parancsStr = "";
            try
            {
                Random r = new Random();
                //int k = r.Next(1000, 9000);
                String query = "INSERT INTO " +
                               "`talalkozo`(`tanacsado_id`, `ugyfel_id`, `datum`, `idopont`, `idotartam`) " +
                               "VALUES((select tanacsado_id from befektetes.tanacsado where nev = '" + comboBox1.Text + "'), " +
                               "(select ugyfel_id from befektetes.ugyfel where nev = '" + comboBox2.Text + "'), " +
                               "'" + dateTimePicker1.Text + "'," + "'" + dateTimePicker2.Text + "','" + numericUpDown1.Value +"')";

                MySqlCommand command = new MySqlCommand(query, kapcsolat);

                command.ExecuteNonQuery();
                MessageBox.Show("Sikeres adat felvétel!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatok betöltésekor! " + ex + " " + parancsStr);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("") || comboBox2.Text.Equals(""))
            {
                MessageBox.Show("Töltsél ki minden adatott!!");

            }
            else
            {
                UjAdatFelvetele();
            }
        }
    }
}
