using identity.Data;
using identity.Models;
using identity.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace identity.Controllers
{
    [Authorize(Roles ="Admin,hr")]
     public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly object userManage;

        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole>roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                    // Check if the role "User" exists
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        // Create the role if it does not exist
                        var roleResult = await roleManager.CreateAsync(new IdentityRole("User"));

                        // Check if the role creation succeeded
                        if (!roleResult.Succeeded)
                        {
                            return View(model);
                        }
                    }

                    // Assign the "User" role to the newly created user
                    await userManager.AddToRoleAsync(user, "User");

                    return RedirectToAction("Login");
                }

            

            return View(model);
        }
        [AllowAnonymous]

        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]

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

        public async Task<IActionResult> GetUsers()
        {
            var users = userManager.Users.ToList();

            var userViewModels = new List<UsersViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                var userViewModel = new UsersViewModel
                {
                    id = user.Id,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    UserName = user.Email,
                    City = user.City,
                    Roles = roles
                };

                userViewModels.Add(userViewModel);
            }

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
        public async Task<IActionResult> EditRole(RoleViewModel model) {
           
            var role = await roleManager.FindByIdAsync(model.Id);

          

            role.Name = model.RoleName;

            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RoleList));      }

       
            return View(model); 
        }


        public ActionResult EditUserRole(string id ) {

            var ViewModel =new EditUserRoleViewModel {  
            Id = id,
            RolesList= roleManager.Roles.Select(
                role=> new SelectListItem {
                Value=role.Id,
                Text=role.Name
                }
                ).ToList(),
            };
            return View(ViewModel);

        }
        [HttpPost]
        public async Task<IActionResult> EditUserRole(EditUserRoleViewModel model) {
            var user = await userManager.FindByIdAsync(model.Id);
            var currentRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, currentRoles);
            var role =await roleManager.FindByIdAsync(model.SelectRoles);
            await userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction(nameof(GetUsers));

        }

        public async Task<IActionResult> Logout() { 
        await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }

}
