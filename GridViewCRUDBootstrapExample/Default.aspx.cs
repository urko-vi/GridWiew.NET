
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewCRUDBootstrapExample
{
    public partial class Default : System.Web.UI.Page
    {
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            BindGrid();                         
        }

        public void BindGrid()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                string cmd = "select * from Region";
                SqlDataAdapter dAdapter = new SqlDataAdapter(cmd, conn);
                DataSet ds = new DataSet();
                dAdapter.Fill(ds);
                dt = ds.Tables[0];
                //Bind the fetched data to gridview
                GridView1.DataSource = dt;
                GridView1.DataBind();
                
            }
            catch (SqlException ex)
            {
                System.Console.Error.Write(ex.Message);

            }  

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("detail"))
            {
               
              //  DetailsView1.DataSource = detailTable;
               // DetailsView1.DataBind();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#detailModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DetailModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("editRecord"))
            {                               
                GridViewRow gvrow = GridView1.Rows[index];                
                lblCodigo.Text = HttpUtility.HtmlDecode(gvrow.Cells[0].Text).ToString();
                txtDescription.Text = HttpUtility.HtmlDecode(gvrow.Cells[1].Text);

                lblResult.Visible = false;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#editModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditModalScript", sb.ToString(), false);
                
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                lblResult.Visible = false;
                string code = GridView1.DataKeys[index].Value.ToString();
                RegionId.Value = code;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteModalScript", sb.ToString(), false);
            }

        }
     

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string code=lblCodigo.Text;
               
            string region = txtDescription.Text;
  
            executeUpdate(code,region);                  
            BindGrid();
            lblError.Text = "Se ha actualizado con exito";
            mensajeAlert.CssClass = "alert alert-success alert-dismissible show";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#mensajeAlert').show();");
            sb.Append("$('#editModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
                
        }

        private void executeUpdate(string code,string countryname)
        {
            string connString = ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;
            try
            {
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                string updatecmd = "update Region RegionDescription=@countryname where RegionId=@code";
                SqlCommand updateCmd = new SqlCommand(updatecmd,conn);                
                updateCmd.Parameters.AddWithValue("@countryname", countryname);

                updateCmd.Parameters.AddWithValue("@code", code);
                updateCmd.ExecuteNonQuery();
                conn.Close();
                
            }
            catch (SqlException me)
            {
                System.Console.Error.Write(me.InnerException.Data);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            lblError.Text = "Se ha actualizado con exito";
            mensajeAlert.CssClass = "alert alert-success alert-dismissible show";
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
            
        }

        protected void btnAddRecord_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text;
            string region = txtRegion.Text;

            executeAdd(code,region);
            BindGrid();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            lblError.Text = "Se ha actualizado con exito";
            mensajeAlert.CssClass = "alert alert-success alert-dismissible ";
            sb.Append(@"<script type='text/javascript'>");

            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
       //     sb.Append("$('#mensajeAlert').modal('show');");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddHideModalScript", sb.ToString(), false);


        }

        private void executeAdd(string code, string region)
        {
            string connString = ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;
            try
            {
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                string insertcmd = "insert into Region (RegionId,RegionDescription) values (@code,@region)";
                SqlCommand addCmd = new SqlCommand(insertcmd, conn);
                addCmd.Parameters.AddWithValue("@code", code);

                addCmd.Parameters.AddWithValue("@region", region);
                addCmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (SqlException me)
            {                
                System.Console.Write(me.Message);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string code= RegionId.Value;

            executeDelete(code);
            BindGrid();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            /*
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("alert('Record deleted Successfully');");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            */
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "delHideModalScript", sb.ToString(), false);


        }

        private void executeDelete(string code)
        {
            string connString = ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;
            try
            {
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                string updatecmd = "delete from Region where RegionId=@code";
                Console.Write(updatecmd);
                SqlCommand addCmd = new SqlCommand(updatecmd, conn);
                addCmd.Parameters.AddWithValue("@code", code);               
                addCmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (SqlException me)
            {
                System.Console.Write("Error"+me.Message);
            }

        }

    }
}