using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cuentas_Bancarias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuentas_Bancarias.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Registro()
    {
        return View("Index");
    }

    [HttpPost("Usuario/registro")]
    public IActionResult ProcesaRegistro(Usuario NuevoUsuario)
    {

        var existeUsuario = _context.Usuarios.FirstOrDefault(u => u.Correo == NuevoUsuario.Correo);

        if (existeUsuario != null)
        {
            ModelState.AddModelError("Email", "El correo ya está registrado.");
            return View("Index");
        }

        if (ModelState.IsValid)
        {
            PasswordHasher<Usuario> Hasher = new();
            NuevoUsuario.Contrasena = Hasher.HashPassword(NuevoUsuario, NuevoUsuario.Contrasena);
            _context.Usuarios.Add(NuevoUsuario);
            _context.SaveChanges();

            HttpContext.Session.SetString("Nombre", NuevoUsuario.Nombre);
            HttpContext.Session.SetString("Apellido", NuevoUsuario.Apellido);
            HttpContext.Session.SetString("Correo", NuevoUsuario.Correo);
            Console.WriteLine(NuevoUsuario.UsuarioId);
            return RedirectToAction("Account", new { usuarioId = NuevoUsuario.UsuarioId });
        }
        return View("Index");
    }

    [HttpPost]
    [Route("Usuario/login")]
    public IActionResult ProcesaLogin(Login login)
    {
        Console.WriteLine(ModelState.IsValid);
        if (ModelState.IsValid)
        {
            Usuario? usuario = _context.Usuarios.FirstOrDefault(usu => usu.Correo == login.Correo);

            if (usuario != null)
            {
                PasswordHasher<Login> Hasher = new();
                var result = Hasher.VerifyHashedPassword(login, usuario.Contrasena, login.Contrasena);

                if (result == PasswordVerificationResult.Success)
                {
                    Console.WriteLine(result);
                    // La contraseña es correcta
                    HttpContext.Session.SetString("Nombre", usuario.Nombre);
                    HttpContext.Session.SetString("Apellido", usuario.Apellido);
                    HttpContext.Session.SetString("Correo", usuario.Correo);
                    HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
                    Console.WriteLine(usuario.UsuarioId);
                    return RedirectToAction("Account", new { usuarioId = usuario.UsuarioId });

                }
                else
                {
                    // Contraseña incorrecta
                    ModelState.AddModelError(string.Empty, "Contraseña incorrecta");
                }
            }
            else
            {
                // Email no registrado
                ModelState.AddModelError("Correo", "El Correo no está registrado");
            }

            return View("Index", usuario);
        }

        return View("Index");
    }

    [HttpGet("Accounts/{usuarioId}")]
    public IActionResult Account(int usuarioId)
    {
        Console.WriteLine("ACATAAAA");
        if (HttpContext.Session.GetInt32("UsuarioId") != usuarioId)
        {
            Console.WriteLine(HttpContext.Session.GetInt32("UsuarioId"));
            return RedirectToAction("Index");
        }

        Usuario? currUser = _context.Usuarios.Include(t => t.ListaTransacciones).FirstOrDefault(u => u.UsuarioId == usuarioId);

        if (currUser != null)
        {
            List<Transaccion> AllTrans = currUser.ListaTransacciones.OrderByDescending(t => t.Fecha_Creacion).ToList();

            if (currUser.ListaTransacciones != null)
            {
                double BalanceActual = currUser.ListaTransacciones.Sum(a => a.Cantidad);
                ViewBag.BalanceActual = BalanceActual.ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                ViewBag.CurrentBalance = "$0.00";
            }

            Console.WriteLine("Balance: " + ViewBag.CurrentBalance);

            ViewBag.ThisUser = currUser;
            ViewBag.AllTransacciones = AllTrans;
        }
        else
        {
            ViewBag.CurrentBalance = "$0.00";
        }

        return View("Accounts");
    }

    [HttpPost("makeTransaction")]
    public IActionResult MakeTransaction(Transaccion NuevaTrans)
    {
        string? balance = Request.Form["CurrBalance"];
        if (balance != null)
        {
            balance = balance.Substring(1);
        double dblBalance = Convert.ToDouble(balance);
        if (dblBalance < NuevaTrans.Cantidad * -1)
        {
            Console.WriteLine("ERROR ON AMOUNT: Cannot withdraw more than your current balance!");
            ModelState.AddModelError("Amount", "Cannot withdraw more than your current balance!");
        }
        else
        {
            Usuario? currUser = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == NuevaTrans.UsuarioId);
            _context.Add(NuevaTrans);
            _context.SaveChanges();
        }
        return Redirect("/Accounts/" + NuevaTrans.UsuarioId);
        }
        return Redirect("index");
    }

    [HttpGet]
    [Route("ProcesaLogout")]
    public IActionResult ProcesaLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        string? email = context.HttpContext.Session.GetString("Email");
        // Check to see if we got back null
        if (email == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "", null);
        }
    }
}
