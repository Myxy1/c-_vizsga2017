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
    public partial class Form2 : Form
    {
        MySqlConnection kapcsolat;
        private DataTable eredmenytabla;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                string kapcsolatString = "SERVER=localhost;" + "DATABASE=befektetes;" + "UID=root;" + "PASSWORD=;";
                kapcsolat = new MySqlConnection(kapcsolatString);
                kapcsolat.Open();
                lista(0);
                elemszam_feltoltes();
                dataGridView1.AllowUserToAddRows = false;
                string[] feliratok = { "Találkozó dátum", "Találkozó kezdése", "Ügyfél neve", "Tanácsadó neve", "Szakterülete", "Találkozó időtartalma", "Tanácsadó óradíja", "Teljes összeg" };
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].HeaderText = feliratok[i];
                }
                /*formázás*/
                dataGridView1.Columns[0].DefaultCellStyle.Format = "yyyy.MM.dd";
                //dataGridView1.Columns[1].DefaultCellStyle.Format = "";
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[6].DefaultCellStyle.Format = "#,##Ft";
                dataGridView1.Columns[7].DefaultCellStyle.Format = "#,##Ft";
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Probléma az adatbázis megnyitásakor! " + ex.Message);
                Close();
            }
        }

        private void lista(int oldal)
        {
            String parancsStr = "";
            try
            {
                parancsStr = "SELECT " +
                            "talalkozo.datum, talalkozo.idopont, ugyfel.nev, tanacsado.nev, szakterulet.megnevezes, talalkozo.idotartam, tanacsado.oradij, " +
                            "(talalkozo.idotartam * tanacsado.oradij) as 'teljes összeg' " +
                            "FROM " +
                            "szakterulet, tanacsado, talalkozo, ugyfel " +
                            "WHERE " +
                            "szakterulet.szakterulet_id = tanacsado.szakterulet_id AND " +
                            "tanacsado.tanacsado_id = talalkozo.tanacsado_id AND " +
                            "talalkozo.ugyfel_id = ugyfel.ugyfel_id " +
                            "ORDER BY " +
                            "`szakterulet`.`megnevezes` ASC, " +
                            "`tanacsado`.`nev` DESC, " +
                            "`talalkozo`.`datum` ASC";
                MySqlCommand parancs = new MySqlCommand(parancsStr, kapcsolat);
                eredmenytabla = new DataTable();
                new MySqlDataAdapter(parancs).Fill(eredmenytabla);  
                dataGridView1.DataSource = eredmenytabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatok betöltésekor! " + ex + " " + parancsStr);
            }
        }
        private void elemszam_feltoltes()
        {
            for (int i = 1; i <= 510; i+=15)
            {
                comboBox1.Items.Add(i);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int x = int.Parse(comboBox1.SelectedItem.ToString());
            dataGridView1.FirstDisplayedScrollingRowIndex = x;
        }
    }
}
