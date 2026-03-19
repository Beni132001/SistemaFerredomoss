using System;
using System.Text.RegularExpressions;

namespace SistemaFerredomos.src.Services
{
    public static class ValidationHelper
    {
        // Texto
        public static bool IsEmpty(string value)
            => string.IsNullOrWhiteSpace(value);

        public static bool ExceedsLength(string value, int maxLength)
            => !string.IsNullOrEmpty(value) && value.Length > maxLength;

        // Números
        public static bool IsPositiveNumber(decimal value)
            => value >= 0;

        public static bool IsValidPrice(decimal value)
            => value >= 0 && value <= 9999999;

        public static bool IsValidStock(decimal value)
            => value >= 0 && value <= 999999;

        public static bool IsValidDouble(string value, out double result)
        {
            result = 0;
            return !string.IsNullOrWhiteSpace(value) &&
                   double.TryParse(value.Replace(",", "."), out result) &&
                   result >= 0;
        }

        // Teléfono
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true; // Opcional
            return Regex.IsMatch(phone.Trim(), @"^[\d\s\-\+\(\)]{7,20}$");
        }

        // Código
        public static bool IsValidCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return true; // Opcional
            return Regex.IsMatch(code.Trim(), @"^[A-Za-z0-9\-_]{1,30}$");
        }

        // Usuario/Contraseña
        public static bool IsValidUsername(string username)
            => !string.IsNullOrWhiteSpace(username) &&
               username.Length >= 3 &&
               username.Length <= 50 &&
               Regex.IsMatch(username, @"^[A-Za-z0-9_\.]+$");

        public static bool IsValidPassword(string password)
            => !string.IsNullOrWhiteSpace(password) && password.Length >= 4;

        // Mensajes de error
        public static string GetNameError(string name, int maxLength = 100)
        {
            if (IsEmpty(name)) return "El nombre es obligatorio.";
            if (ExceedsLength(name, maxLength)) return $"El nombre no puede superar {maxLength} caracteres.";
            return null;
        }

        public static string GetPriceError(decimal value, string fieldName)
        {
            if (value < 0) return $"{fieldName} no puede ser negativo.";
            if (value > 9999999) return $"{fieldName} excede el límite permitido.";
            return null;
        }

        public static string GetStockError(decimal value)
        {
            if (value < 0) return "El stock no puede ser negativo.";
            if (value > 999999) return "El stock excede el límite permitido.";
            return null;
        }

        public static string GetPhoneError(string phone)
        {
            if (!IsValidPhone(phone))
                return "Teléfono inválido. Usa solo números, espacios, guiones o paréntesis.";
            return null;
        }

        public static string GetCodeError(string code)
        {
            if (!IsValidCode(code))
                return "Código inválido. Solo letras, números, guiones o guión bajo.";
            return null;
        }

        public static string GetUsernameError(string username)
        {
            if (IsEmpty(username)) return "El usuario es obligatorio.";
            if (username.Length < 3) return "El usuario debe tener al menos 3 caracteres.";
            if (username.Length > 50) return "El usuario no puede superar 50 caracteres.";
            if (!Regex.IsMatch(username, @"^[A-Za-z0-9_\.]+$"))
                return "El usuario solo puede contener letras, números, puntos o guión bajo.";
            return null;
        }

        public static string GetPasswordError(string password, bool isRequired = true)
        {
            if (isRequired && IsEmpty(password)) return "La contraseña es obligatoria.";
            if (!IsEmpty(password) && password.Length < 4)
                return "La contraseña debe tener al menos 4 caracteres.";
            return null;
        }
    }
}