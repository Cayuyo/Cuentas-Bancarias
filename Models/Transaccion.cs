#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Cuentas_Bancarias.Models;

public class Transaccion {
    [Key]
    public int TransaccionId { get; set;}
    public double Cantidad { get; set; }
    public DateTime Fecha_Creacion { get; set; } = DateTime.Now;
    public int UsuarioId {get;set;}
    public Usuario Creador {get;set;}
    
}