# Descripcion
El Analizador Lexico programa encargado por el profesor Miguel para su clase de Compiladores y interpretadores

#Proposito
Este analizador Lexico se hizo con el objetivo de cumplir mi trabajo en mi clase de Compiladores y interpretadores, no busco que sea un programa a beneficio como programador

#Desarollo
El analizador lexico tiene partes hechas por mi pero pasadas por Inteligencia Artificial asi como modificadas por ella misma.

#Funciones principales que fueron hechas por el Autor:
      private readonly HashSet<string> palabrasReservadas = new HashSet<string>
      private readonly Dictionary<string, string> operadores = new Dictionary<string, string>
      private readonly Dictionary<string, string> simbolos = new Dictionary<string, string>
      public AnalizadorLexicoC(string codigo)
      private void ManejarComentarioLinea()
      private void ManejarEspacios()
      private Token ManejarCaracter()
      private Token ManejarOperadorOSimbolo()
      
      [Algunas de las funciones anteriromente dadas las aprendi en mi curso de C en mis primeros años, ya que analizabamos palabras y las separabamos por tokens]
      
#Funciones implementados con asistencia de IA
  private Token ManejarIdentificador() [Pregunta que propuse para esto "¿Como puedo hacer que mi analizador lexico reconozca una palabra como si fuera un token? No quiero que solo identifique letras si no palabras que vayan deacuerdo al diccionario"]
   private Token ManejarNumero()
    private Token ManejarCadena() [Pregunta que propuse para esto "¿Como ademas de indentificar una palabra puedo hacer que identifique una cadena de caracteres?"]
    private void AgregarError(int line, int column, string mensaje) [Este fue añadido por la misma IA]
    private void btnAnalizar_Click(object sender, EventArgs e) [El boton fue añadido por el autor pero la dificultad era que llamara a todas las demas funciones, tuve muchos errores al intentarlo asi que tuve asistencia por ella misma]

#Nota Legal:
Este proyecto solamente es educativo y no infringe derechos de autor. Las implementaciones de IA fueron utilizadas con responsabilidad y no como suplemente de el programador.
