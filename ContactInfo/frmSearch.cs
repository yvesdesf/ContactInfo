using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ContactInfo
{
    public partial class frmSearch : Form
    {
        public List<Contact> ContactList = new List<Contact>();
        public int Ind = -1;
        public frmSearch()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Contact Name
            dgSearch.Visible = true;
            dgSearch.ColumnCount = 3;
            dgSearch.Columns[0].Name = "Index";
            dgSearch.Columns[1].Name = "FirstName";
            dgSearch.Columns[2].Name = "lastName";
            dgSearch.Rows.Clear();
            dgSearch.Refresh();

            var ContactL = from Contact in ContactList
                           where Contact.lName.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant())
                                            select Contact;
            

            foreach (Contact OneContact in ContactL)
            {
                Debug.Print(OneContact.lName);
                string[] row = new string[] { OneContact.Index.ToString(), OneContact.fName, OneContact.lName };
                dgSearch.Rows.Add(row);

            }

            ContactL = from Contact in ContactList
                           where Contact.fName.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant())
                           select Contact;


            foreach (Contact OneContact in ContactL)
            {
                Debug.Print(OneContact.lName);
                string[] row = new string[] { OneContact.Index.ToString(), OneContact.fName, OneContact.lName };
                dgSearch.Rows.Add(row);

            }

            if (dgSearch.RowCount > 0)
            {
                dgSearch.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dgSearch.Visible = true;
            }

        }

        private void dgSearch_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Ind = Mod.MakeInt(dgSearch.Rows[e.RowIndex].Cells[0].Value);
            this.Close();
        }
    }
}
