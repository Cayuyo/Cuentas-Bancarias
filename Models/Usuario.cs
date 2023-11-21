#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Cuentas_Bancarias.Models;

public class Usuario
{
    [Key]
    public int UsuarioId { get; set; }
    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [MinLength(2, ErrorMessage = "El Nombre debe tener al menos 2 caracteres.")]
    [StringLength(45, ErrorMessage = "El Nombre debe tener como máximo 45 caracteres.")]
    public string Nombre { get; set; } = null!;
    [Required(ErrorMessage = "El Apellido es obligatorio.")]
    [MinLength(2, ErrorMessage = "El Apellido debe tener al menos 2 caracteres.")]
    [StringLength(45, ErrorMessage = "El Apellido debe tener como máximo 45 caracteres.")]
    public string Apellido { get; set; } = null!;
    [Required(ErrorMessage = "El Correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El Correo no es válido.")]
    [StringLength(45, ErrorMessage = "El Correo debe tener como máximo 45 caracteres.")]
    public string Correo { get; set; } = null!;
    [Required(ErrorMessage = "La Contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [StringLength(255)]
    public string Contrasena { get; set; } = null!;
    [NotMapped]
    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
    [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
    public string Rep_Contrasena { get; set; } = null!;
    public DateTime Fecha_Creacion { get; set; }
    public DateTime Fecha_Modificacion { get; set; }

    public List<Transaccion> ListaTransacciones { get; set; } = new List<Transaccion>();

    public Usuario()
    {
        Fecha_Creacion = DateTime.Now;
        Fecha_Modificacion = DateTime.Now;
        FormatearFechas();

    }
    private void FormatearFechas()
    {
        string formatoChileno = "dd-MM-yyyy HH:mm:ss";
        Fecha_Creacion = DateTime.ParseExact(Fecha_Creacion.ToString(formatoChileno), formatoChileno, CultureInfo.InvariantCulture);
        Fecha_Modificacion = DateTime.ParseExact(Fecha_Modificacion.ToString(formatoChileno), formatoChileno, CultureInfo.InvariantCulture);
    }

    public void HashContrasena()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Contrasena));
            Contrasena = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}