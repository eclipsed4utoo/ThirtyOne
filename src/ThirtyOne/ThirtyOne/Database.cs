using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using System.Windows;
using System.Reflection;
using System.Configuration;

namespace ThirtyOne
{
    public class Database
    {
        public void CheckForDatabase()
        {
            string path = string.Empty;

#if !DEBUG
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThirtyOne");
            path = Path.Combine(path, "ThirtyOne.sdf");
#else
            path = Path.Combine(Directory.GetCurrentDirectory(), "ThirtyOne.sdf");
#endif

            if (File.Exists(path))
            {
                DatabaseUpdates();
                return;
            }

            // file doesn't exist.

            try
            {
                using (SqlCeEngine engine = new SqlCeEngine(@"Data Source=|DataDirectory|\ThirtyOne.sdf"))
                {
                    engine.CreateDatabase();
                }

                CreateCustomersTable();
                CreateOrdersTable();
                CreatePartiesTable();
                CreatePaymentTypesTable();
                CreateTableContraints();
                CreatePaymentTypeData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DatabaseUpdates()
        {
            
        }

        private void CreatePaymentTypeData()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "INSERT INTO [PaymentTypes] ([PaymentType],[PaymentTypeID]) VALUES (N'None','c0a5d459-00b2-4ca1-a436-3a57744551e4');";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "INSERT INTO [PaymentTypes] ([PaymentType],[PaymentTypeID]) VALUES (N'Cash','8c2229c7-46d6-4dd7-b70f-fd53ae5bfbcc');";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "INSERT INTO [PaymentTypes] ([PaymentType],[PaymentTypeID]) VALUES (N'Check','4598225c-1175-46d1-8f62-dbdb2a859c3d');";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "INSERT INTO [PaymentTypes] ([PaymentType],[PaymentTypeID]) VALUES (N'Debit/Credit','8a7c971e-091b-4e4e-9255-2cfafb9f405f');";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }

        private void CreateTableContraints()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "ALTER TABLE [Customers] ADD CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerID]);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "ALTER TABLE [Orders] ADD CONSTRAINT [PK__Orders__00000000000000BB] PRIMARY KEY ([OrderID]);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "ALTER TABLE [Parties] ADD CONSTRAINT [PK_Parties] PRIMARY KEY ([PartyID]);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "ALTER TABLE [PaymentTypes] ADD CONSTRAINT [PK__PaymentTypes__00000000000000DE] PRIMARY KEY ([PaymentTypeID]);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "CREATE UNIQUE INDEX [UQ__Customers__0000000000000115] ON [Customers] ([CustomerID] ASC);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "CREATE UNIQUE INDEX [UQ__Orders__00000000000000A9] ON [Orders] ([OrderID] ASC);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "CREATE UNIQUE INDEX [UQ__Parties__000000000000012B] ON [Parties] ([PartyID] ASC);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "CREATE UNIQUE INDEX [UQ__PaymentTypes__00000000000000D6] ON [PaymentTypes] ([PaymentTypeID] ASC);";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "ALTER TABLE [Orders] ADD CONSTRAINT [CustomerOrders] FOREIGN KEY ([CustomerID]) REFERENCES [Customers]([CustomerID]) ON DELETE NO ACTION ON UPDATE NO ACTION;";
                te.ExecuteStoreCommand(script, new object[] { });
                script = "ALTER TABLE [Orders] ADD CONSTRAINT [PaymentTypeOrders] FOREIGN KEY ([PaymentTypeID]) REFERENCES [PaymentTypes]([PaymentTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }

        private void CreatePaymentTypesTable()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "CREATE TABLE [PaymentTypes] ([PaymentType] nvarchar(100) NULL, [PaymentTypeID] uniqueidentifier NOT NULL  ROWGUIDCOL);";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }

        private void CreatePartiesTable()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "CREATE TABLE [Parties] ([PartyID] uniqueidentifier NOT NULL  ROWGUIDCOL, [PartyDate] datetime NOT NULL, [PartyTotal] float NOT NULL);";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }

        private void CreateOrdersTable()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "CREATE TABLE [Orders] ([OrderSubTotal] float NULL, [OrderTax] float NULL, [OrderTotal] float NULL, [IsPaid] bit NOT NULL, [OrderID] uniqueidentifier NOT NULL  ROWGUIDCOL, [CustomerID] uniqueidentifier NOT NULL, [PaymentTypeID] uniqueidentifier NOT NULL, [OrderDate] datetime NOT NULL, [PartyID] uniqueidentifier NULL, [OrderShipping] float NULL);";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }

        private void CreateCustomersTable()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "CREATE TABLE [Customers] ([CustomerID] uniqueidentifier NOT NULL  ROWGUIDCOL, [CustomerName] nvarchar(150) NOT NULL);";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }

        private void CreateTestTable()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                string script = "CREATE TABLE [Test] ([CustomerID] uniqueidentifier NOT NULL  ROWGUIDCOL, [CustomerName] nvarchar(150) NOT NULL);";
                te.ExecuteStoreCommand(script, new object[] { });
            }
        }
    }
}
