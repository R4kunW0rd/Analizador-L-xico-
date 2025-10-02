using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analizador_Lexico
{
    
    public partial class Form1 : Form
    {
        private string codigoFuente = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }
        public class Token
        {
            public string Tipo { get; set; }
            public string Valor { get; set; }
            public int Linea { get; set; }
            public int Columna { get; set; }

            public Token(string tipo, string valor, int linea, int columna)
            {
                Tipo = tipo;
                Valor = valor;
                Linea = linea;
                Columna = columna;
            }
        }

        public class AnalizadorLexicoC
        {
            private string codigoFuente;
            private int posicion;
            private int linea;
            private int columna;
            private StringBuilder errores;

            // Palabras reservadas de C
            private readonly HashSet<string> palabrasReservadas = new HashSet<string>
        {
            "auto", "break", "case", "char", "const", "continue", "default",
            "do", "double", "else", "enum", "extern", "float", "for", "goto",
            "if", "int", "long", "register", "return", "short", "signed",
            "sizeof", "static", "struct", "switch", "typedef", "union",
            "unsigned", "void", "volatile", "while"
        };

            // Operadores y símbolos
            private readonly Dictionary<string, string> operadores = new Dictionary<string, string>
        {
            {"+", "OPERADOR_SUMA"},
            {"-", "OPERADOR_RESTA"},
            {"*", "OPERADOR_MULTIPLICACION"},
            {"/", "OPERADOR_DIVISION"},
            {"%", "OPERADOR_MODULO"},
            {"=", "OPERADOR_ASIGNACION"},
            {"==", "OPERADOR_IGUALDAD"},
            {"!=", "OPERADOR_DIFERENTE"},
            {"<", "OPERADOR_MENOR"},
            {">", "OPERADOR_MAYOR"},
            {"<=", "OPERADOR_MENOR_IGUAL"},
            {">=", "OPERADOR_MAYOR_IGUAL"},
            {"&&", "OPERADOR_AND"},
            {"||", "OPERADOR_OR"},
            {"!", "OPERADOR_NOT"},
            {"&", "OPERADOR_BIT_AND"},
            {"|", "OPERADOR_BIT_OR"},
            {"^", "OPERADOR_BIT_XOR"},
            {"~", "OPERADOR_BIT_NOT"},
            {"<<", "OPERADOR_DESPLAZAMIENTO_IZQ"},
            {">>", "OPERADOR_DESPLAZAMIENTO_DER"},
            {"++", "OPERADOR_INCREMENTO"},
            {"--", "OPERADOR_DECREMENTO"},
            {"+=", "OPERADOR_SUMA_ASIGNACION"},
            {"-=", "OPERADOR_RESTA_ASIGNACION"},
            {"*=", "OPERADOR_MULTIPLICACION_ASIGNACION"},
            {"/=", "OPERADOR_DIVISION_ASIGNACION"},
            {"%=", "OPERADOR_MODULO_ASIGNACION"},
            {"&=", "OPERADOR_AND_ASIGNACION"},
            {"|=", "OPERADOR_OR_ASIGNACION"},
            {"^=", "OPERADOR_XOR_ASIGNACION"},
            {"<<=", "OPERADOR_DESPLAZAMIENTO_IZQ_ASIGNACION"},
            {">>=", "OPERADOR_DESPLAZAMIENTO_DER_ASIGNACION"}
        };

            private readonly Dictionary<string, string> simbolos = new Dictionary<string, string>
        {
            {"(", "PARENTESIS_IZQ"},
            {")", "PARENTESIS_DER"},
            {"{", "LLAVE_IZQ"},
            {"}", "LLAVE_DER"},
            {"[", "CORCHETE_IZQ"},
            {"]", "CORCHETE_DER"},
            {";", "PUNTO_COMA"},
            {",", "COMA"},
            {".", "PUNTO"},
            {":", "DOS_PUNTOS"},
            {"?", "INTERROGACION"},
            {"#", "NUMERAL"}
        };

            public int ErroresCount { get; private set; }

            public AnalizadorLexicoC(string codigo)
            {
                codigoFuente = codigo;
                posicion = 0;
                linea = 1;
                columna = 1;
                errores = new StringBuilder();
                ErroresCount = 0;
            }

            public (List<Token> Tokens, string Errores) Analizar()
            {
                List<Token> tokens = new List<Token>();
                errores.Clear();
                ErroresCount = 0;

                while (posicion < codigoFuente.Length)
                {
                    char caracterActual = codigoFuente[posicion];

                    if (char.IsWhiteSpace(caracterActual))
                    {
                        ManejarEspacios();
                    }
                    else if (caracterActual == '/' && posicion + 1 < codigoFuente.Length && codigoFuente[posicion + 1] == '/')
                    {
                        ManejarComentarioLinea();
                    }
                    else if (caracterActual == '/' && posicion + 1 < codigoFuente.Length && codigoFuente[posicion + 1] == '*')
                    {
                        ManejarComentarioBloque();
                    }
                    else if (char.IsLetter(caracterActual) || caracterActual == '_')
                    {
                        tokens.Add(ManejarIdentificador());
                    }
                    else if (char.IsDigit(caracterActual))
                    {
                        tokens.Add(ManejarNumero());
                    }
                    else if (caracterActual == '"')
                    {
                        tokens.Add(ManejarCadena());
                    }
                    else if (caracterActual == '\'')
                    {
                        tokens.Add(ManejarCaracter());
                    }
                    else
                    {
                        tokens.Add(ManejarOperadorOSimbolo());
                    }
                }

                return (tokens, errores.ToString());
            }

            private void ManejarEspacios()
            {
                while (posicion < codigoFuente.Length && char.IsWhiteSpace(codigoFuente[posicion]))
                {
                    if (codigoFuente[posicion] == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                    else
                    {
                        columna++;
                    }
                    posicion++;
                }
            }

            private void ManejarComentarioLinea()
            {
                int lineaInicio = linea;
                int columnaInicio = columna;

                while (posicion < codigoFuente.Length && codigoFuente[posicion] != '\n')
                {
                    posicion++;
                    columna++;
                }
            }

            private void ManejarComentarioBloque()
            {
                int lineaInicio = linea;
                int columnaInicio = columna;

                posicion += 2; // Saltar "/*"
                columna += 2;

                while (posicion < codigoFuente.Length - 1)
                {
                    if (codigoFuente[posicion] == '*' && codigoFuente[posicion + 1] == '/')
                    {
                        posicion += 2;
                        columna += 2;
                        return;
                    }

                    if (codigoFuente[posicion] == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                    else
                    {
                        columna++;
                    }
                    posicion++;
                }

                // Si llegamos aquí, el comentario no se cerró
                AgregarError(lineaInicio, columnaInicio, "Comentario de bloque no cerrado");
            }

            private Token ManejarIdentificador()
            {
                int inicioLinea = linea;
                int inicioColumna = columna;
                StringBuilder lexema = new StringBuilder();

                while (posicion < codigoFuente.Length &&
                      (char.IsLetterOrDigit(codigoFuente[posicion]) || codigoFuente[posicion] == '_'))
                {
                    lexema.Append(codigoFuente[posicion]);
                    posicion++;
                    columna++;
                }

                string valor = lexema.ToString();
                string tipo = palabrasReservadas.Contains(valor) ? "PALABRA_RESERVADA" : "IDENTIFICADOR";

                return new Token(tipo, valor, inicioLinea, inicioColumna);
            }

            private Token ManejarNumero()
            {
                int inicioLinea = linea;
                int inicioColumna = columna;
                StringBuilder lexema = new StringBuilder();
                bool tienePunto = false;

                while (posicion < codigoFuente.Length &&
                      (char.IsDigit(codigoFuente[posicion]) || codigoFuente[posicion] == '.'))
                {
                    if (codigoFuente[posicion] == '.')
                    {
                        if (tienePunto)
                        {
                            AgregarError(linea, columna, "Número con múltiples puntos decimales");
                            break;
                        }
                        tienePunto = true;
                    }

                    lexema.Append(codigoFuente[posicion]);
                    posicion++;
                    columna++;
                }

                string tipo = tienePunto ? "NUMERO_REAL" : "NUMERO_ENTERO";
                return new Token(tipo, lexema.ToString(), inicioLinea, inicioColumna);
            }

            private Token ManejarCadena()
            {
                int inicioLinea = linea;
                int inicioColumna = columna;
                StringBuilder lexema = new StringBuilder();

                lexema.Append(codigoFuente[posicion]); // Agregar comilla inicial
                posicion++;
                columna++;

                while (posicion < codigoFuente.Length && codigoFuente[posicion] != '"')
                {
                    if (codigoFuente[posicion] == '\\')
                    {
                        // Manejar secuencias de escape
                        lexema.Append(codigoFuente[posicion]);
                        posicion++;
                        columna++;

                        if (posicion < codigoFuente.Length)
                        {
                            lexema.Append(codigoFuente[posicion]);
                            posicion++;
                            columna++;
                        }
                    }
                    else if (codigoFuente[posicion] == '\n')
                    {
                        AgregarError(linea, columna, "Cadena no cerrada en la misma línea");
                        break;
                    }
                    else
                    {
                        lexema.Append(codigoFuente[posicion]);
                        posicion++;
                        columna++;
                    }
                }

                if (posicion < codigoFuente.Length && codigoFuente[posicion] == '"')
                {
                    lexema.Append(codigoFuente[posicion]); // Agregar comilla final
                    posicion++;
                    columna++;
                }
                else
                {
                    AgregarError(inicioLinea, inicioColumna, "Cadena no cerrada");
                }

                return new Token("CADENA", lexema.ToString(), inicioLinea, inicioColumna);
            }

            private Token ManejarCaracter()
            {
                int inicioLinea = linea;
                int inicioColumna = columna;
                StringBuilder lexema = new StringBuilder();

                lexema.Append(codigoFuente[posicion]); // Agregar comilla simple inicial
                posicion++;
                columna++;

                while (posicion < codigoFuente.Length && codigoFuente[posicion] != '\'')
                {
                    if (codigoFuente[posicion] == '\\')
                    {
                        // Manejar secuencias de escape
                        lexema.Append(codigoFuente[posicion]);
                        posicion++;
                        columna++;

                        if (posicion < codigoFuente.Length)
                        {
                            lexema.Append(codigoFuente[posicion]);
                            posicion++;
                            columna++;
                        }
                    }
                    else if (codigoFuente[posicion] == '\n')
                    {
                        AgregarError(linea, columna, "Carácter no cerrado en la misma línea");
                        break;
                    }
                    else
                    {
                        lexema.Append(codigoFuente[posicion]);
                        posicion++;
                        columna++;
                    }
                }

                if (posicion < codigoFuente.Length && codigoFuente[posicion] == '\'')
                {
                    lexema.Append(codigoFuente[posicion]); // Agregar comilla simple final
                    posicion++;
                    columna++;
                }
                else
                {
                    AgregarError(inicioLinea, inicioColumna, "Carácter no cerrado");
                }

                return new Token("CARACTER", lexema.ToString(), inicioLinea, inicioColumna);
            }

            private Token ManejarOperadorOSimbolo()
            {
                int inicioLinea = linea;
                int inicioColumna = columna;

                // Verificar operadores de 2 caracteres primero
                if (posicion + 1 < codigoFuente.Length)
                {
                    string dosCaracteres = codigoFuente.Substring(posicion, 2);
                    if (operadores.ContainsKey(dosCaracteres))
                    {
                        posicion += 2;
                        columna += 2;
                        return new Token(operadores[dosCaracteres], dosCaracteres, inicioLinea, inicioColumna);
                    }
                }

                // Verificar operadores o símbolos de 1 carácter
                string unCaracter = codigoFuente[posicion].ToString();
                if (operadores.ContainsKey(unCaracter))
                {
                    posicion++;
                    columna++;
                    return new Token(operadores[unCaracter], unCaracter, inicioLinea, inicioColumna);
                }
                else if (simbolos.ContainsKey(unCaracter))
                {
                    posicion++;
                    columna++;
                    return new Token(simbolos[unCaracter], unCaracter, inicioLinea, inicioColumna);
                }
                else
                {
                    // Carácter no reconocido
                    AgregarError(inicioLinea, inicioColumna, $"Carácter no reconocido: '{unCaracter}'");
                    posicion++;
                    columna++;
                    return new Token("ERROR", unCaracter, inicioLinea, inicioColumna);
                }
            }

            private void AgregarError(int line, int column, string mensaje)
            {
                errores.AppendLine($"Línea {line}, Columna {column}: {mensaje}");
                ErroresCount++;
            }
        }
        private void btnAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Archivos C (*.c)|*.c|Todos los archivos (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    codigoFuente = File.ReadAllText(openFile.FileName);
                    txtCodigo.Text = codigoFuente;
                    this.Text = $"Analizador Léxico - {Path.GetFileName(openFile.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al leer el archivo: {ex.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Primero abre un archivo o escribe código.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            codigoFuente = txtCodigo.Text;

            try
            {
                AnalizadorLexicoC analizador = new AnalizadorLexicoC(codigoFuente);
                var (tokens, errores) = analizador.Analizar();

                // Limpiar y mostrar resultados
                dataGridView1.Rows.Clear();

                int indice = 1;
                foreach (var token in tokens)
                {
                    dataGridView1.Rows.Add(token.Tipo, token.Valor, token.Linea, token.Columna, indice++);
                }

                // Actualizar título
                this.Text = $"Analizador Léxico - {tokens.Count} tokens, {analizador.ErroresCount} errores";

                // Mostrar errores si existen
                if (!string.IsNullOrEmpty(errores))
                {
                    MessageBox.Show("Errores encontrados:\n" + errores,
                                  "Errores Léxicos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el análisis: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
