using MongoDB.Bson;
using MongoBaseRepository;
using MongoBaseRepository.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ProjectLeader.Models;
using ProjectLeader.ViewModels;

namespace ProjectLeader.Controllers
{
    public partial class AccountController: Controller
    {
        protected readonly IMongoRepository Db;
        protected readonly IMongoMembershipService MemberShip;
        protected ObjectId AuthUserId
        {
          get
          {
            MongoUser user = GetAuthUser();
            return user != null ? user._id : ObjectId.Empty;
          }
        }

        protected MongoUser GetAuthUser()
        {
          if (System.Web.HttpContext.Current.User != null) return Db.GetItem<MongoUser>(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name);
          return null;
        }

        public AccountController(IMongoRepository db = null, IMongoMembershipService _memb = null)
        {
          Db = db ?? DependencyResolver.Current.GetService<IMongoRepository>();
          MemberShip = _memb ?? DependencyResolver.Current.GetService<IMongoMembershipService>();
		}

		[HttpGet]
        public virtual ActionResult LogOn()
        {
          return View(new LoginViewModel() { ProjectName = "ProjectLeader" });
        }

        [HttpPost]
        public virtual ActionResult LogOn(int? id)
        {
            LoginViewModel lvm = new LoginViewModel();
            TryUpdateModel<LoginViewModel>(lvm);
            if (MemberShip.ValidateUser(lvm.Password, lvm.LoginName))
            {
                FormsAuthentication.SetAuthCookie(lvm.LoginName, true);
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("ErrLogOn");
        }

        public virtual ActionResult ErrLogOn()
        {
            return View();
        }

        public virtual ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateAppUser(bool isAdmin, bool isSection)
        {
            return View("Register", new UserCreateUserRegistration() { IsSection = isSection, IsAdmin = isAdmin });
        }

        [HttpPost]
        public ActionResult Register(UserCreateUserRegistration model)
        {
            if (Db.GetItem<MongoUser>(x=> x.UserName == model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "UserName not unique");
                return RedirectToAction("CreateAppUser", model);
            }
            if (ModelState.IsValid)
            {
              //TODO
              MongoUser user = GetUserFromModel(model);
              Db.AddItem<MongoUser>(user, AuthUserId);
            }
            return RedirectToAction("Index", "Home", new { username = model.UserName, isAdmin = model.IsAdmin });
        }

        public MongoUser GetUserFromModel(UserCreateUserRegistration model)
        {
          MongoUser user = new MongoUser();
          return user;
        }

        
        public ActionResult DeleteUser(string username, ObjectId id)
        {
          if (String.IsNullOrEmpty(username))
          {
            Db.RemoveItem<MongoUser>(x=> x._id == id);
          }
          else Db.RemoveItem<MongoUser>(x => x.UserName == username);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditUser()
        {
            return View(new UserCreateUserRegistration());
        }

        [HttpPost]
        public ActionResult SaveUser(UserCreateUserRegistration model)
        {
          if (ModelState.IsValid)
          {
            MongoUser user = GetUserFromModel(model);
            Db.UpdateItem<MongoUser>(x => x.UserName == user.UserName, user);
            return View("Index");
          }
          return View("EditUser", model);
        }
    }
}
