//UTS Banking System//
//Written By Thomas Good - 13291455//
//16/09/2019//

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using System.Net.Mail;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace Assignment1
{
    class Account
    {
        String fName;
        String lName;
        String address;
        long phone;
        String email;
        Boolean v;
        String accNum = "10000";
        int accBalance = 0;
        int fileNumber = 0;
        string fileName = @"Accounts/100001.txt";

        public Account()
        {
            create();
        }

        public void create()        //--Creates new account--//
        {
            int n = 0;
            int m = 0;
            Console.WriteLine("Create New Account");
            Console.WriteLine("Enter the details");
            Console.Write("First Name: ");
            fName = Console.ReadLine();
            Console.Write("Last Name: ");
            lName = Console.ReadLine();
            Console.Write("Address: ");
            address = Console.ReadLine();
            while (n != 1)
            {
                try
                {
                    Console.Write("Phone: (+61)");
                    phone = Convert.ToInt64(Console.ReadLine());
                    double count = Math.Ceiling(Math.Log10(phone));

                                                            //--Checks phone number length and if the number is valid--//

                    if (count > 9)
                    {
                        Console.WriteLine("Phone number invalid: ");
                        Console.WriteLine("Phone number cannot be more than 10 numbers"); 
                    }
                    else if (count < 6)
                    {
                        Console.WriteLine("Phone number invalid: ");
                        Console.WriteLine("Phone number cannot be less than 6 numbers"); 
                    }
                    else
                    {
                        n = 1;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Phone number invalid: ");
                    Console.WriteLine("Please enter a number between 6 and 10");
                }
            }

            while (m != 1)
            {
                Console.Write("Email: ");
                email = Console.ReadLine();
                if (email.Contains("@"))                //--Checks if email input has '@' symbol--//
                {
                    
                    m = 1;

                } else
                {
                    Console.WriteLine("Invalid Email, Please Try Again.");
                }
                
            }

        }

        public Account(String fName, String lName, String address, long phone, String email, String accNum, int accBalance, Boolean v)
        {
            this.fName = fName;
            this.lName = lName;
            this.address = address;
            this.phone = phone;
            this.email = email;
            this.accNum = accNum;
            this.accBalance = accBalance;
            this.v = v;
        }
            
        public void DisplayAccount()            //--Displays account details--//
        {
            Console.Clear();
            Console.WriteLine("First Name: {0}", fName);
            Console.WriteLine("Last Name: {0}", lName);
            Console.WriteLine("Address: {0}", address);
            Console.WriteLine("Phone Number: 0{0}", phone);
            Console.WriteLine("Email: {0}", email);
        }

        public Boolean ConfirmAccount(bool v)       //--Confirms Account details--//
        {
            Console.Write("Is the information correct? (y/n): ");
            String answer = Console.ReadLine();
            if (answer == "y" || answer == "Y" || answer == "Yes" || answer == "yes") 
            {
                v = true;
            } else if (answer == "n" || answer == "N" || answer == "No" || answer == "no")
            {
                v = false;
            } else
            {
                Console.WriteLine("Response not valid");
                v = false;
            }
            return v;
        }

        public void checkFile()         //--Iterates through files to check which is the next available account number--//
        {
            bool exists = true;
            while (exists == true) {

                fileNumber = fileNumber + 1;
                fileName = @"Accounts/10000" + fileNumber + ".txt";

                if (File.Exists(fileName))
                {
                    checkFile();
                }
                else
                {
                    createFile();

                    exists = false;
                }
                break;
            }
        }

        public void createFile()            //--Writes to a new file with account number as title--//
        {
            String accountNumber = accNum + fileNumber;
            String tran = @"Accounts/" + accountNumber + "-transaction.txt";
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine("Account Number: {0}", accountNumber);
                sw.WriteLine("Account Balance: {0}", accBalance);
                sw.WriteLine("First Name: {0}", fName);
                sw.WriteLine("Last Name: {0}", lName);
                sw.WriteLine("Address: {0}", address);
                sw.WriteLine("Phone: 0{0}", phone);
                sw.WriteLine("Email: {0}", email);
                sw.Close();
                sw.Dispose();
            }

            using (StreamWriter tr = File.CreateText(tran))     //--Creates transaction file--//
            {
                tr.Close();
                tr.Dispose();
            }

        }

        public void ConfirmAccount()    //--If account confirmation comes back as yes, send account details to email provided--//
        {
            if (ConfirmAccount(true))
            {
                checkFile();

                try
                {


                    string[] lines = File.ReadAllLines(fileName);
                    foreach (string line in lines)
                    {
                        if (line.Contains("Email: "))
                        {
                            email = line.Substring(7);
                        }
                    }

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("utsbanksystememail@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Account Details";

                    Attachment attachment;
                    attachment = new Attachment(fileName);
                    mail.Attachments.Add(attachment);
                    mail.Body = "Account Details is attached below";

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new NetworkCredential("utsbanksystememail@gmail.com", "UTSBankSystemEmail");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);

                    SmtpServer.Dispose();
                    mail.Dispose();
                    attachment.Dispose();
                    



                    Console.WriteLine("Account created! Details will be provided via email");
                    Console.WriteLine("Account number is: 10000{0}", fileNumber);

                    Console.ReadKey();
                    fileNumber = 1;

                } catch (FormatException e)         //--If invalid email provided, ask to retry--//
                {
                    File.Delete(fileName);
                    fileNumber = 1;
                    Console.Clear();
                    Console.WriteLine("Invalid email. Please try again.");
                    create();
                    ConfirmAccount();

                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Please re-enter your account details");
                create();
                ConfirmAccount();

            }
        }
    }

    class MainClass
    {
        static int answer;
        static int i = 0;
        static String email = "";

        public static void Main(string[] args)
        {
            Login();
        }

        public static void Menu()           //--Main Menu--//
        {
            Console.Clear();
            Console.WriteLine("Welcome to Simple Banking System");
            Console.WriteLine("=========================================");
            Console.WriteLine("1. Create a new account");
            Console.WriteLine("2. Search for an account");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Withdraw");
            Console.WriteLine("5. A/C statement");
            Console.WriteLine("6. Delete account");
            Console.WriteLine("7. Exit");
            Console.WriteLine("=========================================");
            Console.Write("Enter your choice (1-7): ");
            try
            {
                answer = Convert.ToInt32(Console.ReadLine());
            } catch (FormatException e)
            {
                Menu();
            }
            if (answer == 1)
            {
                Console.Clear();
                CreateAccount();
            }
            else if (answer == 2)
            {
                Console.Clear();
                Search();
            }
            else if (answer == 3)
            {
                Console.Clear();
                Deposit();
            }
            else if (answer == 4)
            {
                Console.Clear();
                Withdraw();
            }
            else if (answer == 5)
            {
                Console.Clear();
                Statement();
            }
            else if (answer == 6)
            {
                Console.Clear();
                Delete();
            }
            else if (answer == 7)
            {
                Environment.Exit(0);
            }
            else
            {
                Menu();
            }
        }

        public static void Login()      //--Login using username/password from login.txt file--//
        {
            Console.WriteLine("Welcome to Simple Banking System");
            Console.WriteLine("Login to start");
            Console.Write("Username: ");
            String inputUser = Console.ReadLine();
            SecureString pass = hidePass();
            var inputPass = new NetworkCredential(string.Empty, pass).Password;
            String loginText = @"login.txt";
            StringBuilder newFile = new StringBuilder();
            string[] file = File.ReadAllLines(loginText);

            foreach (string line in file)
            {
                if (line.Contains(inputUser))
                {
                    int index = inputUser.Length;
                    String password = line.Substring(index + 1);
                    if (inputPass == password)
                    {
                        Console.WriteLine("\nValid Credentials, Please enter");
                        Console.ReadKey();
                        Console.Clear();
                        Menu();
                    } else
                    {
                        if (i < 2)
                        {
                            Console.WriteLine("\nFailed to login, please try again");
                            i++;
                            Console.ReadKey();
                            Console.Clear();
                            Login();

                        }
                        else
                        {
                            Console.WriteLine("Too many failed attempts, exiting now");
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }

        public static void CreateAccount()              //--Creates account object--//
        {
            Account acc = new Account();
            acc.DisplayAccount();
            acc.ConfirmAccount();
            Menu();
        }

        public static void Search()     //--Searches account based on account number--//
        {
            Console.Clear();
            Console.WriteLine("Search An Account");
            Console.WriteLine("Enter the details");
            Console.Write("Account Number: ");
            String num = @"Accounts/" + Console.ReadLine() + ".txt";

            if (File.Exists(num))
            {
                Console.WriteLine("Account found!");
                string[] lines = File.ReadAllLines(num);
                foreach (string line in lines)
                {
                    Console.WriteLine("\t" + line);
                }
                if (ConfirmSearch(true))
                {
                    Search();
                } else
                {
                    Menu();
                }
            } else
            {
                Console.WriteLine("Account not found");
                if (ConfirmSearch(true))
                {
                    Search();
                }
                else
                {
                    Menu();
                }
            }
        }

        public static void Deposit()            //--Will search for account, deposit input amount - Adding to the account balance--//
        {
            Console.Clear();
            Console.WriteLine("Deposit");
            Console.WriteLine("Enter the details");
            Console.Write("Account Number: ");
            String acc = Console.ReadLine();
            String num = @"Accounts/" + acc + ".txt";
            String transaction = @"Accounts/" + acc + "-transaction.txt";
            
            if (File.Exists(num))               //--Checks if account exists--//    
            {
                Console.Write("Amount: $");
                String deposit = Console.ReadLine();
                StringBuilder newFile = new StringBuilder();
                string temp = "";

                    string[] file = File.ReadAllLines(num);

                    foreach (string line in file)
                    {

                        if (line.Contains("Account Balance: "))         //--Replaces line with old acccount balance with new account balance--//
                    {
                            try
                            {
                                String sub = line.Substring(17);
                                double old = Convert.ToDouble(sub);
                                double total = old + Convert.ToDouble(deposit);
                                temp = line.Replace(line.Substring(0), "Account Balance: " + total);
                                newFile.Append(temp + "\r\n");
                                continue;
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Please enter an amount to deposit");
                                Console.ReadKey();
                                Deposit();
                            }
                        }
                        newFile.Append(line + "\r\n");
                    
                    
                }

                File.WriteAllText(num, newFile.ToString());

                int x = 1;
           

                string[] lines = File.ReadAllLines(transaction);        //--Iterates through transaction history to display latest 5 transactions--//
                foreach (string line in lines)
                {
                    if (x >= 5)
                    {
                        var tLines = File.ReadAllLines(transaction).Skip(1);
                        File.WriteAllLines(transaction, tLines);
                    }
                    x++;
                }

                using (StreamWriter sw = File.AppendText(transaction))      //--Appends deposited amount to transaction file--//
                {
                    sw.WriteLine("Deposit: $" + deposit);
                    sw.Close();
                }
                Console.WriteLine("Deposit Successful!");
                Console.ReadKey();
                Menu();

            }
            else
            {
                Console.WriteLine("Account not found");
                if (ConfirmSearch(true))
                {
                    Deposit();
                }
                else
                {
                    Menu();
                }
            }
        }

        public static void Withdraw()       //--Will search for account, withdraw input amount - Removing from the account balance--//
        {
            Console.Clear();
            Console.WriteLine("Withdraw");
            Console.WriteLine("Enter the details");
            Console.Write("Account Number: ");

            String acc = Console.ReadLine();
            String num = @"Accounts/" + acc + ".txt";
            String transaction = @"Accounts/" + acc + "-transaction.txt";

            if (File.Exists(num))
            {
                Console.Write("Amount: $");
                String withdraw = Console.ReadLine();
                StringBuilder newFile = new StringBuilder();
                string temp = "";
                string[] file = File.ReadAllLines(num);
                foreach (string line in file)
                {
                    if (line.Contains("Account Balance: ")) //--Replaces line with old acccount balance with new account balance--//
                    {
                        try
                        {
                            String sub = line.Substring(17);
                            double old = Convert.ToDouble(sub);
                            double amount = Convert.ToDouble(withdraw);
                            
                            if (amount > old)
                            {
                                Console.WriteLine("Insufficient funds!");
                                if (ConfirmSearch(true))
                                {
                                    Withdraw();
                                }
                                else
                                {
                                    Menu();
                                }
                            }
                            
                            double total = old - amount;
                            temp = line.Replace(line.Substring(0), "Account Balance: " + total);
                            newFile.Append(temp + "\r\n");
                            continue;
                        } catch (FormatException e)
                        {
                            Console.WriteLine("Please enter an amount to withdraw");
                            Console.ReadKey();
                            Withdraw();
                        }
                    }
                    newFile.Append(line + "\r\n");
                }

                File.WriteAllText(num, newFile.ToString());
                int x = 1;
                string[] lines = File.ReadAllLines(transaction);    //--Iterates through transaction history to display latest 5 transactions--//
                foreach (string line in lines)
                {
                    if (x >= 5)
                    {
                        var tLines = File.ReadAllLines(transaction).Skip(1);
                        File.WriteAllLines(transaction, tLines);
                    }
                    x++;
                }

                using (StreamWriter sw = File.AppendText(transaction))
                {
                    sw.WriteLine("Withdraw: $" + withdraw);         //--Appends withdrawn amount to transaction file--//
                    sw.Close();
                }
                Console.WriteLine("Withdraw Successful!");
                Console.ReadKey();
                Menu();

            }
            else
            {
                Console.WriteLine("Account not found");
                if (ConfirmSearch(true))
                {
                    Withdraw();
                }
                else
                {
                    Menu();
                }
            }
        }

        public static void Statement()      //--Will provide statement of account based on given account number--//
                                                            //--Will display 5 latest transactions--//
        {
            Console.Clear();                        
            Console.WriteLine("Statement");
            Console.WriteLine("Enter the details");
            Console.Write("Account Number: ");
            String acc = Console.ReadLine();
            String num = @"Accounts/" + acc + ".txt";
            String transaction = @"Accounts/" + acc + "-transaction.txt";
            String final = @"Accounts/" + acc + "-statement.txt";

            if (File.Exists(num))               //--Checks if account exists--//
            {
                Console.WriteLine("Account found!");
                string[] lines = File.ReadAllLines(num);
                foreach (string line in lines)
                {
                    if (line.Contains("Email: "))
                    {
                        email = line.Substring(7);
                    }
                    using (StreamWriter sw = File.AppendText(final))
                    {
                        sw.WriteLine(line);
                    }
                    Console.WriteLine("\t" + line);
                }
                Console.WriteLine("================Latest Transactions================");

                string[] tLines = File.ReadAllLines(transaction);       
                foreach (string line in tLines)
                {       
                    using (StreamWriter sw = File.AppendText(final))    //--Creates temporary 'final' statement file consisting of account details and transaction history--//
                    {   
                        sw.WriteLine(line);
                        sw.Close();
                    }
                    Console.WriteLine("\t" + line);

                }

                if (ConfirmStatement(true))
                {

                    MailMessage mail = new MailMessage();                                   //--If confirmation comes back true, send final statement to email provided --//
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("utsbanksystememail@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Bank Statement";

                    Attachment attachment;
                    attachment = new Attachment(final);
                    mail.Attachments.Add(attachment);
                    mail.Body = "Bank Statement is attached below";

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new NetworkCredential("utsbanksystememail@gmail.com", "UTSBankSystemEmail");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);

                    SmtpServer.Dispose();
                    mail.Dispose();
                    attachment.Dispose();

                    Console.WriteLine("Email sent successfully");
                    File.Delete(final);
                    Console.ReadKey();
                    Menu();
                }
                else
                {
                    File.Delete(final);
                    Menu();
                }
            }
            else
            {
                Console.WriteLine("Account not found");
                if (ConfirmSearch(true))
                {
                    Statement();
                }
                else
                {
                    Menu();
                }
            }
        }

        public static void Delete()     //--Will search for account and delete if confirmed--//
        {
            Console.Clear();
            Console.WriteLine("Delete an account");
            Console.WriteLine("Enter the details");
            Console.Write("Account Number: ");
            String acc = Console.ReadLine();
            String num = @"Accounts/" + acc + ".txt";
            String transaction = @"Accounts/" + acc + "transaction.txt";

            if (File.Exists(num))           //--Checks if account exists--//
            {
                Console.WriteLine("Account found!");
                string[] lines = File.ReadAllLines(num);
                foreach (string line in lines)
                {
                    Console.WriteLine("\t" + line);
                }
                if (ConfirmDelete(true))
                {
                    File.Delete(num);               //--Deletes account file and transaction file--//
                    File.Delete(transaction);
                    Console.WriteLine("Account Deleted");
                    Console.ReadKey();
                    Menu();
                }
                else
                {
                    Menu();
                }
            }
            else
            {
                Console.WriteLine("Account not found");
                if (ConfirmSearch(true))                //--Asks if user would like to try another account--//
                {   
                    Search();
                }
                else
                {
                    Menu();
                }
            }
        }

        public static Boolean ConfirmDelete(bool v)         //--Account Deletion confirmation--//
        {
            Console.Write("Delete (y/n)? ");
            String ans = Console.ReadLine();
            if (ans == "y" || ans == "Y" || ans == "Yes" || ans == "yes")
            {
                v = true;
            }
            else if (ans == "n" || ans == "N" || ans == "No" || ans == "no")
            {
                v = false;
            }
            else
            {
                Console.WriteLine("Response not valid");
                v = false;
            }
            return v;
        }

        public static Boolean ConfirmStatement(bool v)      //--Account Statement confirmation--//
        {
            Console.Write("Email Statement (y/n)? ");
            String ans = Console.ReadLine();
            if (ans == "y" || ans == "Y" || ans == "Yes" || ans == "yes")
            {
                v = true;
            }
            else if (ans == "n" || ans == "N" || ans == "No" || ans == "no")
            {
                v = false;
            }
            else
            {
                Console.WriteLine("Response not valid");
                v = false;
            }
            return v;
        }

        public static Boolean ConfirmSearch(bool v)         //--Account Search confirmation--//
        {
            Console.Write("Check another account (y/n)? ");
            String ans = Console.ReadLine();
            if (ans == "y" || ans == "Y" || ans == "Yes" || ans == "yes")
            {
                v = true;
            }
            else if (ans == "n" || ans == "N" || ans == "No" || ans == "no")
            {
                v = false;
            }
            else
            {
                Console.WriteLine("Response not valid");
                v = false;
            }
            return v;
        }

        public static SecureString hidePass()           //--Hides password input when entering at login--//
        {
            Console.Write("Password: ");
            SecureString pass = new SecureString();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    pass.AppendChar(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass.RemoveAt(pass.Length - 1);
                    Console.Write("\b \b");
                }
                
            }
            while (keyInfo.Key != ConsoleKey.Enter);
            {
                return pass;
            }
        }
    }
}
