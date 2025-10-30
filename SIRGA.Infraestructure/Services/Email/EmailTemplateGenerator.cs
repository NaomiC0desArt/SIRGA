
using SIRGA.Application.Interfaces.Services.Email;

namespace SIRGA.Infraestructure.Services.Email
{
    public class EmailTemplateGenerator: IEmailTemplateGenerator
    {
        public string GenerateWelcomeEmail(
            string firstName,
            string email,
            string temporaryPassword,
            string role)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{ 
            max-width: 600px; 
            margin: 0 auto; 
            padding: 20px;
        }}
        .header {{ 
            background-color: #4a5759; 
            color: white; 
            padding: 30px 20px; 
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
        }}
        .content {{ 
            padding: 30px 20px; 
            background-color: #f8f9fa;
        }}
        .credentials {{ 
            background-color: white; 
            padding: 20px; 
            margin: 20px 0; 
            border-left: 4px solid #4a5759;
            border-radius: 5px;
        }}
        .credentials h3 {{
            margin-top: 0;
            color: #4a5759;
        }}
        .credentials p {{
            margin: 10px 0;
        }}
        .credentials .value {{
            font-family: 'Courier New', monospace;
            background-color: #f0f0f0;
            padding: 5px 10px;
            border-radius: 3px;
            display: inline-block;
        }}
        .warning {{ 
            background-color: #fff3cd;
            color: #856404;
            padding: 15px;
            border-left: 4px solid #ffc107;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .steps {{
            background-color: white;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .steps ol {{
            padding-left: 20px;
        }}
        .steps li {{
            margin: 10px 0;
        }}
        .footer {{ 
            text-align: center; 
            padding: 20px; 
            font-size: 12px; 
            color: #666;
            background-color: #e9ecef;
            border-radius: 0 0 5px 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 Bienvenido a SIRGA</h1>
        </div>
        
        <div class='content'>
            <h2>Hola {firstName},</h2>
            <p>Se ha creado una cuenta para ti en el <strong>Sistema de Registro y Gestión Académica (SIRGA)</strong>.</p>
            <p><strong>Tu rol:</strong> {role}</p>
            
            <div class='credentials'>
                <h3>📧 Tus credenciales de acceso:</h3>
                <p><strong>Email:</strong><br/><span class='value'>{email}</span></p>
                <p><strong>Contraseña temporal:</strong><br/><span class='value'>{temporaryPassword}</span></p>
            </div>

            <div class='warning'>
                <strong>⚠️ IMPORTANTE:</strong> Debes cambiar tu contraseña en el primer inicio de sesión por seguridad.
            </div>

            <div class='steps'>
                <h3>📋 Próximos pasos:</h3>
                <ol>
                    <li>Ingresa al sistema con tus credenciales</li>
                    <li>Completa la información de tu perfil</li>
                    <li>Cambia tu contraseña temporal por una segura</li>
                </ol>
            </div>

            <p>Si tienes algún problema o pregunta, contacta al administrador del sistema.</p>
            
            <p>¡Bienvenido a bordo!</p>
        </div>
        
        <div class='footer'>
            <p>Este es un mensaje automático, por favor no responder.</p>
            <p>&copy; 2025 SIRGA - Sistema de Registro y Gestión Académica</p>
        </div>
    </div>
</body>
</html>
            ";
        }

       
        public string GeneratePasswordResetEmail(string firstName, string resetUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            margin: 0;
            padding: 0;
        }}
        .container {{ 
            max-width: 600px; 
            margin: 0 auto; 
            padding: 20px;
        }}
        .header {{ 
            background-color: #c1121f; 
            color: white; 
            padding: 30px 20px; 
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
        }}
        .content {{ 
            padding: 30px 20px; 
            background-color: #f8f9fa;
        }}
        .button-container {{
            text-align: center;
            margin: 30px 0;
        }}
        .button {{ 
            display: inline-block; 
            padding: 15px 30px; 
            background-color: #c1121f; 
            color: white; 
            text-decoration: none; 
            border-radius: 5px;
            font-weight: bold;
            font-size: 16px;
        }}
        .button:hover {{
            background-color: #780000;
        }}
        .url-box {{
            background-color: white;
            padding: 15px;
            margin: 20px 0;
            border: 1px solid #ddd;
            border-radius: 5px;
            word-break: break-all;
            font-size: 12px;
            color: #666;
        }}
        .warning {{
            background-color: #fff3cd;
            color: #856404;
            padding: 15px;
            border-left: 4px solid #ffc107;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .footer {{ 
            text-align: center; 
            padding: 20px; 
            font-size: 12px; 
            color: #666;
            background-color: #e9ecef;
            border-radius: 0 0 5px 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Restablecer Contraseña</h1>
        </div>
        
        <div class='content'>
            <h2>Hola {firstName},</h2>
            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en SIRGA.</p>
            
            <p>Haz clic en el siguiente botón para crear una nueva contraseña:</p>
            
            <div class='button-container'>
                <a href='{resetUrl}' 
   class='button' 
   style='display: inline-block; padding: 15px 30px; background-color: #c1121f; color: #ffffff !important; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;'>
    Restablecer Contraseña
</a>
            </div>

            <p><small>O copia y pega este enlace en tu navegador:</small></p>
            <div class='url-box'>
                {resetUrl}
            </div>

            <div class='warning'>
                <strong>⏱️ Este enlace expirará en 1 hora.</strong>
            </div>

            <p>Si no solicitaste este cambio, ignora este mensaje y tu contraseña permanecerá sin cambios.</p>
            
            <p><strong>Por seguridad:</strong> Nunca compartas tu contraseña con nadie.</p>
        </div>
        
        <div class='footer'>
            <p>Este es un mensaje automático, por favor no responder.</p>
            <p>&copy; 2025 SIRGA - Sistema de Registro y Gestión Académica</p>
        </div>
    </div>
</body>
</html>
            ";
        }

        
        public string GenerateEmailConfirmationEmail(string firstName, string confirmationUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{ 
            max-width: 600px; 
            margin: 0 auto; 
            padding: 20px;
        }}
        .header {{ 
            background-color: #28a745; 
            color: white; 
            padding: 30px 20px; 
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
        }}
        .content {{ 
            padding: 30px 20px; 
            background-color: #f8f9fa;
        }}
        .button-container {{
            text-align: center;
            margin: 30px 0;
        }}
        .button {{ 
            display: inline-block; 
            padding: 15px 30px; 
            background-color: #28a745; 
            color: white; 
            text-decoration: none; 
            border-radius: 5px;
            font-weight: bold;
            font-size: 16px;
        }}
        .button:hover {{
            background-color: #218838;
        }}
        .url-box {{
            background-color: white;
            padding: 15px;
            margin: 20px 0;
            border: 1px solid #ddd;
            border-radius: 5px;
            word-break: break-all;
            font-size: 12px;
            color: #666;
        }}
        .footer {{ 
            text-align: center; 
            padding: 20px; 
            font-size: 12px; 
            color: #666;
            background-color: #e9ecef;
            border-radius: 0 0 5px 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✉️ Confirma tu Email</h1>
        </div>
        
        <div class='content'>
            <h2>Hola {firstName},</h2>
            <p>Gracias por registrarte en SIRGA. Por favor, confirma tu dirección de correo electrónico haciendo clic en el botón de abajo:</p>
            
            <div class='button-container'>
                <a href='{confirmationUrl}' 
   class='button' 
   style='display: inline-block; padding: 15px 30px; background-color: #28a745; color: #ffffff !important; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;'>
    Confirmar Email
</a>
            </div>

            <p><small>O copia y pega este enlace en tu navegador:</small></p>
            <div class='url-box'>
                {confirmationUrl}
            </div>

            <p>Si no creaste esta cuenta, puedes ignorar este mensaje de forma segura.</p>
        </div>
        
        <div class='footer'>
            <p>Este es un mensaje automático, por favor no responder.</p>
            <p>&copy; 2025 SIRGA - Sistema de Registro y Gestión Académica</p>
        </div>
    </div>
</body>
</html>
            ";
        }
    }
}
