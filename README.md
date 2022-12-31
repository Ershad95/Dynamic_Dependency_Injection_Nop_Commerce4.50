#### Dynamic_Dependency_Injection_Nop_Commerce4.50
  سلام <a href="https://ershad95.github.io/">ارشاد رئوفی</a> هستم - اگه یادتون باشه در رابطه با تزریق داینامیک وابستگی ها یه سری مطلب گذاشته بودم و روی آپارات هم به صورت کامل توضیح دادم حالا  داستان تزریق وابستگی داینامیک بدون نوشتن کد های تکراری روی سورس  <a href="https://www.nopcommerce.com/">Nop Commerce</a> پیاده سازی کردم و واقعا خوب جواب داده! با من باشید جادوگری ها داریم :) 
## مراحل پیاده سازی : 
 اول از همه یک اینترفیس به اسم ICustomService ایجاد کردم که صرفا یک Prop به اسم InjectType داره و مشخصی میکنه به چه صورتی این سرویس به ServiceCollection تزریق بشه 
 اما مهم تر از اینکه تزریق مشخص کنه -مشخص میکنه چه سرویس هایی توسط ما اضافه شدن و با سرویس های خود nop تفکیک میشه حالا چه جوری ؟؟؟
### قانون اول : 
 برای هر سرویسی که می نویسیم و میخوایم DI رو روش اعماال کنیم باید Interface  ICustomService رو به ارث برده باشه دقیقا به خاطر دو دلیللی که بالا بهش اشاره شد.
### قانون دوم :‌ 
 هر کلاسی که میاد و Interface مرتبط به سرویس ها رو پیاده سازی میکنه باید prop InjectType رو در سازنده خودش مقداردهی کنه تا متوجه بشیم از چه طریقی باید تزریق انجام بشه 
 تا اینجا یه چارچوب مشخص کردیم تا سرویس ها رو بتونیم تشخیص بدیم اما هنوز کار اصلی مونده !!!
## تعیین نقطه شروع :
 باید نقطه شروع به کار Nop پیدا کنیم اینم مدونیم که نقطه شروع Dot Net Core قست StartUp هست ولی میخواییم با معماری Nop جلو بریم پس با یکم بررسی و دیدن کد ها به یه کلاسی میرسیم به اسم NopStartup در قسمت Nop.Web.Framework - مسیر دقیقش ‌:‌ Nop.Web.Framework\Infrastructure\NopStartup.cs
 حالا این کلاس چیه ؟؟؟ در واقع هر کلاسی که از سرویس INopStartup ارث بری کرده باشه اولویت پیدا میکنه و قبل از کدهای دیگه اجرا میشه به همین سادگی!
 باید یه کلاس جدید به اسم مثلا CustomDependencyInjection ایجاد کنیم با این تفاوت که حتما از کلاس NopStartup ارث بری کرده باشه و نکته خیلی مهم این باید متدی به اسم ConfigureServices رو override کنه 
 حالا داخل متدی که گقتم بایدشروع کنیم به جادو گری!!!
## تحلیل کدهای متد ConfigureServices :
######   //-------------Get All Services-------------
######            var asm = AppDomain.CurrentDomain
######                 .GetAssemblies()
######                 .Single(x => x.FullName.Contains("Nop.Services"));

 در این کد اسمبلی قسمت Nop.Service برمیداریم چرا؟؟ چون تمام سرویس هایی که می نویسیم در این قسمت قرار داره اگر خدایی نکرده خواستید سرویس هاتون رو جای دیگه ای بنویسد باید اسم Nop.Service به چیزی که میخوایید تغییر بدید من تطبق استاندارد جلو رفتم پس شما هم قانون مند باشید !
######  //-------------find Services that inheriance of ICustomService-------------
######            var types = asm.DefinedTypes.Where(x => IsSubInterface(x, typeof(ICustomService))); 
 یادتونه گقتم باید مشخص بشه چه سرویس هایی رو ما مشخص کردیم ! اینجا وقتشه ازش استفاده کنیم و سرویس های خودمون رو تفکیک کنیم
######  //-----------Get All Custom Service Classess-------
######            var allRelatedClassServices = types.Where(x => x.IsClass);
######            //-----------Get All Custom Service Interfaces-------
######            var allRelatedInterfaceServices = types.Where(x => x.IsInterface);
 تو این مرحله تمام کلاس ایمنترفیس هایی که شخصی سازی کردیم رو به دست میاریم 
###### //-----------Matche Class Services To Related Interface Services-------
######            foreach (var classService in allRelatedClassServices)
######            {
######                //-----------get related interface for service class-----------
######                var interfaceService = allRelatedInterfaceServices.Single(x => x.Name == $"I{classService.Name}");           
######            }
 اینم از مرحله آخر یعنی ارتباط دادن سرویس ها کلاس و اینترفیس های کلاس 
## قانون سوم :  
کلاس ها و اینترفیس های سرویس باید یک نام گذاری واحد داشته باشند برای این قانون از استاندارد خود Nop که خیلی منطقی هم هست استفاده کردم یعنی :
#### Class Name : Entitiy_Name + "Service"
#### Interface Name : "I"+Entitiy_Name + "Service"
با استفاده از همین الگو میتونم خیلی راحت سرویس و اینترفیس مرتبط به هم وصل کنم
حالا به چه روشی تزریق کنیم ؟؟؟ 
یادتونه که یک prop داشتیم به اسم Inject وقتشه ازش استفاده کنیم 

<pre>
                var InjectValue = (InjectType)classService.GetProperty("Inject")
                   .GetValue(Activator.CreateInstance(classService), null);
                //----------finally Add Custom Service To Service Collection-----------
               switch (InjectValue)
                {
                    case InjectType.Scopped:
                        services.AddScoped(interfaceService, classService);
                        break;
                    case InjectType.Transit:
                        services.AddTransient(interfaceService, classService);
                        break;
                    case InjectType.SingleTon:
                        services.AddSingleton(interfaceService, classService);
                        break;
                    default:
                        break;
                }
</per>
 
یادتون باشه این کلاس صرفا یکبار در حیات نرم افزار اجرا میشه و قرار نیست در طی جرخف نرم افزار این متذ با کدهایی که گقتم اجرا بشن پس نگران هزینه بربودن یا اشغال شدن منابع سیستمی نباشید :)

#### لینک آپارت من : https://aparat.com/ershad74
#### لینک سایت من : https://ershad95.github.io
