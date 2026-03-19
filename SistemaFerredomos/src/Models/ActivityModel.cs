using System;

namespace SistemaFerredomos.src.Models
{
    public class ActivityModel
    {
        public int      Id          { get; set; }
        public int      UserId      { get; set; }
        public string   UserName    { get; set; }
        public string   Activity    { get; set; }
        public int?     ReferenceId { get; set; }
        public DateTime DateTime    { get; set; }

        public string ActivityLabel => Activity switch
        {
            "login"           => "🔑 Inicio de sesión",
            "logout"          => "🚪 Cierre de sesión",
            "orden"           => "📋 Orden creada",
            "proveedor_orden" => "📦 Pedido a proveedor",
            _                 => Activity
        };
    }
}