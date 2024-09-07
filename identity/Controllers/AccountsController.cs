using identity.Data;
using identity.Models;
using identity.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace identity.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole>roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    UserName = model.Email,
                    City = model.City,
                    Gender = model.Gender,
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(GetUsers));
                }
            }

            return View(model);
        }

        public IActionResult GetUsers()
        {
            var users = userManager.Users.ToList();

            var userViewModels = users.Select(x => new UsersViewModel
            {
                Email = x.Email,
                Phone = x.PhoneNumber,
                UserName = x.Email,
                City = x.City
            }).ToList(); 

            return View(userViewModels);
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            
                IdentityRole role = new IdentityRole
                {
                    Name = model.RoleName
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(RoleList));
                }


            

            return View(model);
        }

        public IActionResult RoleList() {
            var roles=roleManager.Roles.ToList();
            var x=roles.Select(x => new RoleViewModel {
                Id = x.Id,
                RoleName = x.Name,
            }).ToList();
            return View(x);

                }
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction(nameof(RoleList));
            }

            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RoleList));
            }

           
            return RedirectToAction(nameof(RoleList));
        }

        public async Task<ActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

           
            var model = new RoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
           
            var role = await roleManager.FindByIdAsync(model.Id);

          

            role.Name = model.RoleName;

            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RoleList));      }

       
            return View(model); 
        }
    }
}
