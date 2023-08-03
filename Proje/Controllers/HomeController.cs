using Proje.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Proje.Models.Model;

namespace Proje.Controllers
{
    public class HomeController : Controller
    {
        private KurumsalDBContext db=new KurumsalDBContext();
        public ActionResult Index()
        {
            ViewBag.Blog = db.Blog.OrderByDescending(x => x.BlogId);
            return View();
        }
        public ActionResult SliderPartial()
        {
            return View(db.Slider.ToList().OrderByDescending(x=>x.SilderId));
        }
        public ActionResult HizmetPartial()
        {
            return View(db.Hizmet.ToString());
        }
        public ActionResult Hakkimizda() 
        {
            return View(db.Hakkimizda.SingleOrDefault());
        }
        public ActionResult Hizmetlerimiz()
        {
            return View(db.Hizmet.ToList().OrderByDescending(x => x.HizmetId));
        }
        public ActionResult Iletisim() 
        {
            return View(); 
        }
        [HttpPost]
        public ActionResult Iletisim(string adsoyad=null,string email=null,string konu=null, string mesaj=null)
        {
            if (adsoyad != null && email != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "Akifbey2003@gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.Send("Akifbey2003@gmail.com", konu, email + "</br>" + mesaj);
                ViewBag.Uyari = "Mesajınız başarı ile gönderilmiştir.";
            }
            else
            {
                ViewBag.Uyari = "Hata oluştu. Tekrar deneyiniz.";
            }
            return View();
        }
        public ActionResult Blog(int Sayfa=1)
        {
            return View(db.Blog.Include("Kategori").OrderByDescending((x) => x.BlogId).ToPagedList(Sayfa,5));
        }
        public ActionResult KategoriBlog(int id,int Sayfa=1)
        {
            var b = db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).Where(x => x.Kategori.KategoriId == id).ToList().ToPagedList(Sayfa,5);
            return View(b);
        }
        public ActionResult BlogDetay(int id)
        {
            var b=db.Blog.Include("Kategori").Include("Yorums").Where(x=>x.BlogId== id).SingleOrDefault();
            return View(b) ;
        }

        public JsonResult YorumYap(string adsoyad, string eposta, string icerik, int blogid)
        {
            if (icerik==null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Yorum.Add(new Yorum { AdSoyad=adsoyad,Eposta=eposta,Icerik=icerik,BlogId=blogid,Onay=false});
            db.SaveChanges();
            
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BlogKategoriPartial()
        {
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderBy(x=>x.KategoriAd));
        }
        public ActionResult BlogKayitPartial()
        {
            return PartialView(db.Blog.ToList().OrderByDescending(x=>x.BlogId));
        }
    }
}