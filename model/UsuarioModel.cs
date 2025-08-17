using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Restaurante.model
{
    public class UsuarioModel
    {
        [Browsable(false)]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El número de documento es requerido")]
        [MaxLength(20, ErrorMessage = "El número de documento no puede exceder 20 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo {0} debe ser un número.")]
        [Display(Order = 1)]
        public string NoDocumento { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Sexo")]
        public string Sexo { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "La dirección no puede exceder 150 caracteres.")]
        [Display(Name = "Dirección", Order = 3)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "El número de teléfono debe tener entre 10 y 11 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo {0} debe ser un número.")]
        [Display(Order = 4)]
        public string Telefono { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [MaxLength(100, ErrorMessage = "El correo electrónico no puede exceder 100 caracteres.")]
        [Display(Name = "Correo Electrónico", Order = 5)]
        public string Correo { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "El nombre de usuario no puede exceder 100 caracteres.")]
        [Display(Name = "Usuario", Order = 2)]
        public string Usuario { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres.")]
        [Browsable(false)]
        public string Contrasena { get; set; }

        [Display(Name = "Tipo Usuario")]
        public string TipoUsuario { get; set; }

        [Required]
        [Display(Name = "ID Sexo")]
        [Browsable(false)]
        public int? IdSexo { get; set; }

        [Required]
        [Display(Name = "ID Tipo")]
        [Browsable(false)]
        public int? IdTipo { get; set; }

        [Required]
        [Display(Name = "ID Sucursal")]
        [Browsable(false)]
        public int? IdSucursal { get; set; }

        [Display(Name = "Sucursal")]
        public string Sucursal { get; set; }

        [Required]
        [Display(Name = "Estatus", Order = 7)]
        public bool? Estatus { get; set; }

        [Range(0, 100, ErrorMessage = "La comisión debe ser un valor entre 0 y 100 (porcentaje).")]
        [Display(Name = "Comisión por ventas", Order = 8)]
        public decimal? Comision { get; set; } = 0.0m;

        [Required]
        [Display(Name = "Fecha de Creación")]
        [Browsable(false)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Nivel de Acceso", Order = 6)]
        public int? IdNivelAcceso { get; set; }

        public override string ToString()
        {
            return $"ID Usuario: {IdUsuario}, " +
                   $"Número de Documento: {NoDocumento}, " +
                   $"Nombre: {Nombre}, " +
                   $"IdSexo: {IdSexo}, " +
                   $"Dirección: {Direccion}, " +
                   $"Teléfono: {Telefono}, " +
                   $"Correo Electrónico: {Correo}, " +
                   $"IdTipo de Usuario: {IdTipo}, " +
                   $"Estatus: {Estatus}, " +
                   $"Comisión por ventas: {Comision:C2}, " +
                   $"IdSucursal: {IdSucursal}, " +
                   $"Fecha de Creación: {FechaCreacion:d}";
        }
    }
}