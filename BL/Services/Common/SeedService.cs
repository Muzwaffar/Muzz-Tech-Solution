using BL.Models.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Access;
using DAL.Models.Activities;
using DAL.Models.Administration;
using DAL.Models.Common;
using DAL.Models.Form;
using DAL.Models.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BL.Services.Common
{
    public class SeedService
    {
        public static async void Template(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").FirstOrDefault();
            }
            catch (Exception e)
            {

            }

        }

        //Create Roles
        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                if (!roleManager.RoleExistsAsync("Administrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "Administrator";
                    role.NormalizedName = "ADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
                if (!roleManager.RoleExistsAsync("SuperAdministrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "SuperAdministrator";
                    role.NormalizedName = "SUPERADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
            }
            catch (Exception e)
            {

            }

        }

        //Create Users and Assign with Specific roles
        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                if (userManager.FindByNameAsync("admin@muzztech.mu").Result == null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.UserName = "admin@muzztech.mu";
                    user.Email = "admin@muzztech.mu";
                    user.NormalizedUserName = "admin@muzztech.mu";
                    user.NormalizedEmail = "admin@muzztech.mu";
                    user.FirstName = "admin";
                    user.Title = string.Empty;
                    user.LastName = "admin";
                    user.UserCode = "admin";
                    user.UserToken = Guid.Parse(user.Id);
                    user.Status = "AC";
                    user.LoginDetail = new LoginLog()
                    {
                        //  LastKnownIPAddress = clientIp + " ~ " + Request.UserHostAddress.ToString(),
                        LastKnownIPAddress = "192.168.0.1",
                        LastLoginDate = DateTime.Now,
                        Description = "Success First Time Login",
                        userId = user.Id,
                    };


                    IdentityResult result = userManager.CreateAsync
                    (user, "Admin@123.").Result;


                    if (result.Succeeded)
                    {
                        NPF_SYS_User dt = new NPF_SYS_User() { AId = user.Id };
                        context.NPF_SYS_User.Add(dt);
                        context.SaveChanges();
                        userManager.AddToRoleAsync(user, "Administrator").Wait();


                    }
                }
                if (userManager.FindByNameAsync("superadmin@muzztech.mu").Result == null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.UserName = "superadmin@muzztech.mu";
                    user.Email = "superadmin@muzztech.mu";
                    user.NormalizedUserName = "superadmin@muzztech.mu";
                    user.NormalizedEmail = "superadmin@muzztech.mu";
                    user.FirstName = "superadmin";
                    user.Title = string.Empty;
                    user.LastName = "superadmin";
                    user.UserCode = "superadmin";
                    user.UserToken = Guid.Parse(user.Id);
                    user.Status = "AC";
                    user.LoginDetail = new LoginLog()
                    {
                        //  LastKnownIPAddress = clientIp + " ~ " + Request.UserHostAddress.ToString(),
                        LastKnownIPAddress = "192.168.0.1",
                        LastLoginDate = DateTime.Now,
                        Description = "Success First Time Login",
                        userId = user.Id,
                    };


                    IdentityResult result = userManager.CreateAsync
                    (user, "SAdmin@123.").Result;


                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "SuperAdministrator").Wait();
                        NPF_SYS_User dt = new NPF_SYS_User() { AId = user.Id };
                        context.NPF_SYS_User.Add(dt);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedUpdateUsers(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                var applicationUsers = context.Users.ToList();
                foreach (var userM in applicationUsers)
                {
                    if (userM.Language == null || userM.Language == String.Empty)
                    {
                        userM.Language = "English";
                        context.Users.Update(userM);
                        context.SaveChanges();
                    }
                    continue;
                }




            }
            catch (Exception e)
            {

            }

        }

        public static async void SeedEmailConfig(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                if (!context.EmailConfiguration.Any(x => x.CompanyId == -1))
                {
                    EmailConfiguration config = new EmailConfiguration();
                    config.CompanyId = -1;
                    config.Username = "reza.dowlut@muzztech.mu";
                    config.Password = "Nav@Test12345.";
                    config.Port = 587;
                    config.Host = "pro.turbo-smtp.com";
                    context.EmailConfiguration.Add(config);
                    context.SaveChanges();
                }


            }
            catch (Exception e)
            {

            }

        }
        public class ValueSeeding
        {
            public string Name { get; set; }
            public List<string> lst = new List<string>();
        }
        public static async void SeedLookUpType(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                List<NPF_SYS_LookUpType> LTl = new List<NPF_SYS_LookUpType>();
                Guid createdBy = new Guid();
                if (context.Users.Any(x => x.Email.ToLower() == "admin@muzztech.mu"))
                {
                    createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                }

                //Add Name of LookUptype to be seeded in list below
                string[] LookUpList = new string[] { "Title", "Gender", "Status", "Locality", "District", "License Category", "Priority", "Vehicle Category"
                    , "Vehicle Abbreviation" ,"Equipment Category","Equipment Abbreviation", "Fatality Type", "Casualty Type"};
                List<ValueSeeding> LookVall = new List<ValueSeeding>();
                ValueSeeding LookVal = new ValueSeeding();

                //Duplicate within comments to add LookUpvalue Seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Title";//Replace with LookUP Name
                string[] Title = new string[] { "Mr", "Mrs", "Miss", "Prof", "Dr" };// Build List Od=f values
                LookVal.lst.AddRange(Title);
                LookVall.Add(LookVal);
                //Duplicate within comments to add LookUpvalue Seeding
                //Gender seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Gender";
                string[] Gender = new string[] { "Male", "Female" };
                LookVal.lst.AddRange(Gender);
                LookVall.Add(LookVal);
                //Status seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Status";
                string[] Status = new string[] { "Completed", "In Progress", "Draft", "Schedule", "Cancelled", "Unschedule", "Submitted" };
                LookVal.lst.AddRange(Status);
                LookVall.Add(LookVal);
                //Locality seeding 
                LookVal = new ValueSeeding();
                LookVal.Name = "Locality";
                string[] Locality = new string[] { "Beau Bois","Quatre Bords","Latapie","Camp De Masque Pave","Mare Goyaves","Petite Cabane","Camp Ithier","Isidore Rose",
                    "Isidore Rose Pave","Belle Vue Allendy","Boulet Blanc","Boulet Rouge","Camp Garreau","Central Flacq","Cite Argy","Cite Hibiscus","Constance","Domaine La Colombe","Hermitage","La Source",
                    "Morcellement Argy","Morcellement Hospital","Nehru Nagar","Plaine De Gersigny","Riche Mare","Saint Remy","Belle Rose","Clemencia","Ecroignard","Anahita","Beau Champ","Beau Champ Sugar Estate",
                    "Camp Pecheurs","Four Seasons Hotel","Grande River South East","Morcellement VRS","Quartier","Laventure","Camp Creole","Mare La Chaux","Bel Etang","Medine","Mont Ida","Camp Beau Bois","Camp Jakeeree",
                    "CHA","Deep River S.E","EDC","Kewal Nagar","La Nourrice","Louis Renaud","Morcellement Deep River","Morcellement Hamid","Morcellement Trois Ilots","Morcellement VRS Kewal Nagar","Plaines Bananes",
                    "Rousset Road","SILWF - Olivia","Trois Ilots","Bras dEau","Camp Accacia","Camp Poorun","Camp Raphia","Cite Perdu","Cremation","Jardin","La Porte","Morcellement Constance","Morcellement Janoo",
                    "Morcellement Monet","Poste de Flacq","Poste Lafayette","Railway","One and Only St Geran Hotel","Belle Mare","Camp Marcelin","Camp Rannoo","Palmar","Quatre Cocos","Saint Julien Village","Rich Fund",
                    "Union Flacq","Cent Gaulettes","Cite Sebastopol","Clavet","Etoile","Lesur","Pellegrin","Sebastopol","Beau Rivage","Camp Nadal","Camp Nowbuth","Camp Toorab","Debarcadere","Desperoux","La Pelouse","Le Maho",
                    "Montee Bastille","Palmar Beach","Sept Croisees","Touessrock/Casuarina","Victoria","Belvedere","Latapie","Brisee Verdiere","Mare dAustralia","Lalmatie","Mission Cross","Belvedere","Grand Bas Fond",
                    "Bonne Mere","Camp Bonne Mere","NHDC Bonne Mere","Queen Victoria","Bramsthan","Boutique Cocos","Grande Retraite","Bois dOiseaux","Petite Retraite","Bambous Virieux","Debarcadere","Les Mares","Pointe Bambous",
                    "Bananes","Eau Bleue Reservoir","Grand Bel Air","Anse Jonchée","Bois Des Amourettes","Domaine des Chasseurs","Providence","Bemanique","Camp Ramdin","Cluny","Eau Bleue Tostee","Railway","Camp Balma",
                    "Camp Pierre","Grand Sable","Petit Sable","Plaine des Roches","Pointe aux Roches","Allee Gheude","Blue Bay","Carreau Manioc","Cent Gaulettes","Cite La Chaux","Debarcadere","Hospital","Ile aux Deux Cocos",
                    "Maurice Street","Morcellement Lorette","Museum","Pointe Canon","Pointe D'Esny","Pte des Regates","Pointe Jerome","Quartier","Remy Ollier","Tombeau","Ville Noire","Villeneuve","Waterfront","Allee Coco",
                    "Cite EDC","Cite Mare D'Albert","Mare dAlbert","Morcellement Gokool","Morcellement  Grove","Morcellement  Piscine","Morcellement Samlaul","Morcellement Toolsee","Rampe Le Moirt","Camp Francois","Cite Greenwood",
                    "Gros Bois Road","Joli Bois","Jugoo Bhunjun","Mare Tabac","Morcellement SIT","NHDC Mare Tabac","Gros Bois","Astroea Rochecouste","Bamboo","Bois dOiseaux","Business Park","Cite La Rosa","Cite Mont Fertile",
                    "Cite NHDC","Deux Bras","Deux Bras Sugar Estate","Gros Billot","Gros Billot Branch","Kalimaye","La Rosa","Mare Chicose","Mont Fertille","Morcellement Balance","Morcellement  Dreepaul","Morcellement Ganga",
                    "Morcellement Gaytree","Morcellement Mauripark","Morcellement VRS","Parasol","Siding","Bemanique","Bissoondoyal Road","Commelonne","Grand Port Road","Kanpur","La Peyre","Morcellement Beau Climat","NHDC Nouvelle France",
                    "Nouvelle France","Pont Colville","Residence Union.Park","Savanne Road","Seegoolam Road","Belle Vue /Kenya","Cite NHDC","Debarcadere","Ferney","Le Vallon","Vieux Grand Port","Agricultural Extension Office","Balance",
                    "Bois dOiseaux","Cemetry Road","Chatgaon (ex Airport)","Cite Burrenchobay","Cite EDC Balance","La Cambuse","La Grotte","Le Chaland","Madame Dayla","Mon Desert Road","Morcellement  Gobin","Morcellement Ithier",
                    "NHDC les Palmiers","Plaine Magnien","Rishi Dayanand Road","Ruisseau Copeaux","Solitude","SSR International Airport","Camp Carol","Cite CHA","Cite Longtill","Morcellement Ferney VRS","Pte Brocus","Riviere des Creoles",
                    "Baramia","Camp Bouvet","Capitol","Chakla","Chapel","Gebert","Letord","Le Vieux Moulin","Madame Lolo","Marie Jeannie","Mongelard","Mon Rose 1","Mon Rose 2","Morcellement Brito","Morcellement Domaine","Morcellement Jac",
                    "Morcellement Orchidee","Morcellement  Rosiere","Morcellement SIT","Mosque Road","Railway","Residence Mare Chicose","Rose Belle","Beau Site","Camp Falbert","Camp Florine","Camp Jardin","Camp Ramphul","Cent Gaulettes",
                    "Cite CHA","Cite St Hilaire","Hydro Power Station","Le Val Nature Park","Morcellement Dussoye","Morcellement  Henry","Riche en Eau","St Hilaire","St Hubert","Ste Madeleine","Ste Philomene","Beau Fonds","Carreau Esnouf",
                    "Carreau La Paille","Cite Ouvriere","Cite Trois Boutiques","Malakoff","Morcellement  Aspirale","Morcellement Les Palmiers","Morcellement Pitchen","Plein Bois","Trois Boutiques","Union Vale","Virginia","Balisson",
                    "Cluny Road","Morcellement Cheminee","Morcellement VRS","School Road","Union Park","Cite Padco","Beau Vallon","Cite CHA","Cite NHDC","Morcellement Mahe","Verger","Camp Vinson","Cemetery","Petit Bel Air","Camp Carol",
                    "Carreau Acacia","Le Bouchon","Mon Desert Mon Tresor","Morcellement VRS Mon Desert","Midlands","Morcellement Ferret","Morcellement Ramdanee","Morcellement St Therese","Cite Anoushka","Cite Mon Bois","Coriolis","Mon Bois",
                    "Morcellement  Domah","Morcellement  Sookary","Morcellement Ramphul 1","Morcellement Ramphul 2","Seizieme Mile","Deux Freres Mountain side","Deux Freres Seaside","Marie Jeanne","Mr Joly","Pointe aux Feuilles","Quatre Soeurs",
                    "Baie Du Cap","Choisy","Ahchin","Akla Road","Bois Cheri","Building","Cader Road","Cite EDC Bois Cheri","Cooperative","Heerallol Road","Mambahal","Morcellement VRS","Porida","Residence Bois Cheri","Seeruttun Road","Tivoli",
                    "EDC Camp Diable","Morcellement Savannah","Ilot","Camp Diable","NHDC Camp Diable","Riche Bois","Adventist Road","Camp Barbe","Camp Charlot","Camp Fanny","Camp Fanny Lorquet","Camp Galets","Camp Goolbar","Camp Jeannette",
                    "Camp Lilas","Florentina","Jooloom","Jumbo","Lotus","Mont Carmel","Morcellement Dunputh","Mosque Road","New Mosque","Plaine des Galets","Playground","Railway","Riviere des Galets","Saint Felix","Satanah",
                    "Social Welfare Centre Road","Allee Jacques","Camp Bananes","College Lane","Fazerally Road","Grand Bois","La Forge Road","Le Grand Road","Media","Morcellement SIT","NHDC Grand Bois","NHDC Le Grand","Old Savanne Road",
                    "Sewsagur Road","Young Tow","Senneville","Mourcellement Rountree","PrudHomme","Morcellement PrudHomme","Maisonette","Railway","Barracks","Camp Martin","La Vanille Crocodile Park","Bel Air Saint Felix","Camp Rabaud",
                    "Camp Siajee","Riviere du Poste","Terrain Maurice","Cite Boodhoo","Cite EDC Chaline","Cite EDC Pitot","Cite Gris Gris","Cite Texamon","Lady Barkly","NHDC Brise de Mer","Morcellement Gris Gris","Souillac","Terracine",
                    "VRS 1 Souillac","VRS 2 Souillac","African Town Squatters","Balance","Cemetary Squatters","Cite de Dieu Squatters","Cite EDC 1 Surinam","Cite EDC 2 Surinam","Coastal Mandir Road","Katamaraz","Martiniere",
                    "Morcellement Dunputh","Morcellement Didi","Morcellement Gobin","NHDC Filao","Pomponette","Rochester/Mosque","Saint Louis","Sept Croisees","Trois Bras","Beau Champ","Bel Ombre","Cite EDC Bel Ombre",
                    "Cite Longtill Bel Ombre","Sainte Marie","Saint Martin","Batimarais","Colmar","Benares","Britannia","Cite Rampur","VRS Britannia","Riviere Dragon","Morcellement VRS Riviere Dragon","Camp Berthaud",
                    "Chamouny","Mare Anguilles","Mont Blanc","Allee Jacques","Camp la Hache","Camp Tagore","Charles Road","Chattorgoon","Cite EDC","Cite Longtill","Cite Paul et Virginie","La Barraque Sugar Estate","La Chapelle","LEscalier",
                    "Morcellement Barbe","Morcellement la Sourdine","Morcellement Tagore","Savannah","Savannah Sugar Estate","Union Ducray Sugar Estate & Union Saint Aubin","Cite Saint Aubin","Morcellement Saint Aubin","New Morcellement SIT",
                    "Bostom Road","Cooperative Road","Kanhye Road","La Flora","Matoo Road","Pamplemousses Street","Rioux","Seebaruth Road","Cite Tyack","Lower Tyack","Morcellement Blue Print","NHDC Tyack","Upper Tyack","Morcellement Ah Fouye",
                    "Morcellement Berthaud","Morcellement Bholah","Morcellement Narain","Morcellement Sookia","Stanley 1","Stanley 2","Trefles 1","Trefles 2","Trefles 3","Tefles Housing Estate","Camp Levieux 1","Camp Levieux 2","Cite Corp De Garde",
                    "Espace Concorde","Flat Bhunjun","Flat Roches Brunes","Morcellement Gaya","Morcellement Chady","Morcellement Gopal","Morcellement Nouvelle Ville","Morcellement Seeburn","Morcellement Narain","NHDC Camp Levieux Geranium",
                    "NHDC Camp Levieux Begonia","NHDC Camp Levieux Chrysanteme","NHDC Camp Levieux Dalhia","NHDC Camp Levieux Eglantine","NHDC Camp Levieux Flamboyant","Residence La Concorde","Roche Brunes/ Plaisance","Roche Brunes/ Plaisance 1",
                    "Roche Brunes/ Plaisance 2","Sites and Services","Arab Town","Domaine de LIntendance","Flat Abdoolah","Flat Hollywood","Flat Monmartre","Flat Pem","Flat Poupinel De Valence","Flat Robinson","Galerie Atchia","Galerie Evershine",
                    "Galerie Royale","Govinden Court","Mandarin Court","Market Rose Hill","Mont Royal","Morcellement St Andrews","Morcellement Rogers","NPF Building","Plaisance 1","Plaisance 2","Plaisance Housing Estate","Queen Elizabeth College","Queens Court",
                    "Residence Clos Verger","Residence Malartic","Rose Hill Centre 1","Rose Hill Centre 2","Rose Hill Centre 3","Rose Hill Centre 4","Rose Hill Centre 5","Rose Hill Centre 6","Rose Hill Centre 7","Rose Hill Centre 8","Rose Hill Centre 9","Morcellement Lapeyre",
                    "Brown Sequard Hospital","Central Prison","Cite Barkly","Mont Roches","Morcellement La Comete","Morcellement New Town","Residence St Daniel","Roches Brunes","Verger Bissambar","Morcellement Mount Pleasant","Sunset View","Beau Bassin Centre 1","Beau Bassin Centre 2",
                    "Beau Bassin Centre 3","Beau Bassin Centre 4","Beau Bassin Centre 5","Beau Bassin Centre 6","Beau Bassin Centre 7","Cascadella","Cascadelle","Cite De Rosnay","Cite Vuillemin","Mare Gravier Ward 5","Morcellement Avrillon","NHDC Vuillemin","Vuillemin","Belle Etoile",
                    "Belvedere/Cinquieme Mile","Chebel","Cite Chebel H.E 1","Cite Chebel H.E 2","Cite Chebec","Coromandel","Maingard","Mare Gravier Ward 6","Morcellement Clairmont","Morcellement Hart De Keating","Morcellement Hermitage","Morcellement La Confiance 1",
                    "Morcellement La Confiance 2","Morcellement Lim Fat","Montreal 1","Montreal 2","Montreal 3","Montreal 4","Panorama","Police Flats 1","Industrial Zone Coromandel","NJDC Beryl Barkly","Beau Sejour 1","Beau Sejour 2","Belle Rose 5","Ebene Cybercity","Belle Rose 4",
                    "Candos","Cite St Jean","Dreamton Park","Emerald Park","La Colline Commercial Centre","Le Merit","Ebene Views","Morc Saint Jean","NHDC Villeneuve","Orchard Center","Orchard Tower","Pellegrin","Pierrefonds","Quatre Bornes 1","Quatre Bornes 5","Residence d'Epinay",
                    "Sodnac","Trianon","Trianon Shopping Centre","Victoria Hospital","Residence Trianon","Emerald Park Centre","Le Merit 2","Les Halles","Flat River Islands","Boundary","Cite Beau Sejour/Residence","Cite Candos/Residence","Cite Pere Laval","La Louise","Quatre Bornes 2",
                    "Quatre Bornes 3","Quatre Bornes 4","La Ferme Ave","La Source","Morc Pierrefonds","NHDC Palm","Palma 1","Palma 2","Renganaden Seeneevassen Ave","Bassin/Palma","Bassin SE","Cite Bassin","Cite Kennedy/Residence","Jackson Ave","Morc Cerisier","Morc Dookun 1","Morc Medine",
                    "Morc RIverside","Nehru Road","Palma Junction","Morc Mio","Beard","Camp Belin","Camp Mapou","Camp Roches","Carreau Lalianne","Diolle","Glen Park","NHDC Glen Park","Henrietta","La Marie","Morcellement Pousson","Morcellement Seetaram","Hollyrood No 2","Hollyrood No 1",
                    "La Vanille","Morcellement Munbodh","Morcellement Peerun","Quinze Cantons No 1","Quinze Cantons No 2","Reunion","Sadally","Saint Paul No 1","Thompson","Tres Bon No 1","Tres Bon No 2","Tres Bon No 3","Tres Bon No 4","Tres Bon No 5","Nanapragassen","Bonne Terre No 1","Cite La Caverne",
                    "La Caverne No 1","La Caverne No 2","Modern","Mon Desir No 1","Solferino No 1","Solferino No 2","Solferino No 3","Solferino No 4","Solferino No 5","Vingta No 1","Vingta No 2","Vingta No 3","Visitation","Paillot No 2","Morcellement Dookhun No 2","Allee Brillant","Angus","Clairfonds No 1",
                    "Clairfonds No 2","Clairfonds No 3","Gymkhana","Jackson","Mon Desir No 2","Morcellement Fleuriot","Morcellement Sauba","Riverwalk","SMF Quarters","Trait dUnion","Paillot No 1","Morcellement Peerun No 2","Clarisse House","Bonne Terre No 2","Morcellement Benydin","Allee Jacques","Mesnil",
                    "Morcellement Boucan","Nalletamby","Phoenix 2","Petit Camp","Pont Fer","Saint Paul","Valentina","Valentina Industrial Zone","Phoenix 3","Camp Foquereaux 1","Camp Foquereaux 2","Camp Foquereaux 3","Camp Foquereaux Morcellement Pirthee","Cinq Arpents","Hermitage 1","Mesnil 2",
                    "Camp Foquereaux 6","Morcellement Plaines de LHermitage","Riverside","Castel 1","Allee Brillant 3","Berthaud","Cite EDF","Cite LOiseau","Cite Mangalkhan","De Burg Edwards","John Kennedy","King George V","Morcellement GIDC","Queen Mary","Riviere Seche 1","Simonet","Allee Brillant 1",
                    "Allee Brillant 2","Floreal Square","Riviere Seche 2","King George VI","Morcellement Medine","Engrais Martial 2","Abbe Laval","Bougainville","Camp Pierrot","Edgar Laurent","Gardens","Cite Souchon","Liset Geoffroy","Farquhar","Daruty de Grandpre Road","Morcellement Souchon","Noelville",
                    "Royal College Curepipe","La Mairee","La Peyrouse","Morcellement Piat","Bernardin de St Pierre","Pope Henessy Road","Remono 1","Camp Rouillard","Charles Regnaud","Mayer Region","Morcellement Rosette","Abbe de la Caille","Camp Dhobi","Camp Levieux","Flat Lotus","Cite Malherbes",
                    "Cite Malherbe A","Cite Rivet","Clement Charoux","Couvent de Lorette","Malherbes","Eau Coulee 1","Eau Coulee 2","Engrais Cathan","Thomy DArifat","Morcellement Nassapen","Morcellement Adam","Morcellement Baptiste","Morcellement Camphrieres","Morcellement Cantin","NHDC Malherbe",
                    "Quatre Carreaux","Morcellement Chambly","Notre Dame de Lorette","Camp Bombaye","Camp Lagesse","Camp le Juge","Charles Lees","Cite Atlee","Cite Camp le Juge","Commerson","Cite la Brasserie","Cite St Luc","Flat St Antoine","Icery","Higginson","Jan Palach Shopping Complex","Malartic",
                    "La Croix","Les Aubineaux","Morcellement LOarna","Louis Pasteur","Magnolia Flats","Maurice Martin","R Gujadhur","Morcellement Carbonel","Morcellement Devoyenne","Morcellement Leclezio","Morcellement Piat","Salafa Complex","Market","PSC","Residence la Colombe","Residence les Jasmins",
                    "Rochecouste","Zone Industrielle","Municipality of Curepipe","Celicourt Antelme","La Brasserie South","Morcellement Antelme","Morcellement Pilot","Remono 2","Jan Palach Sud","Queen Elizabeth","Jan Palach North","Le Clezio 2","Barracks","Brown Sequard","Camp Caval","Commerford",
                    "Cite des Versailles","Cite Joachim","Flat Ramdanee","Forest Side 1","Forest Side 2","Les Casernes","Daruty de Grandpre No 2","Robinson 1","Robinson 2","Robinson 3","Botanical Garden","Stevenson","Winston Churchill","Allee Camphre","Haut Plateau","Morcellement Ramalingum",
                    "Mrcellement Hansrod","Ligne Berthaud","Chemin Cimetiere","Camp Thorel","St Julien d'Hotman","L'Assurance","Lower Dagotiere","NHDC Dagotiere","Upper Dagotiere","Valetta","Esperance","Beau Bois","L'Avenir","La Laura","River Baptiste","Malenga","Melrose","NHDC Melrose","Appartment Le Ravin",
                    "Bocage","Bois Cheri","Bon Air(+ villa road)","Camp Samy","Cite Mahatma Gandhi","Cite Telfair","Gentilly","Gentilly Estate (New Morc MDA)","Mauritius Broadcasting Corporation","Moka Business Centre","Moka Eye Hospital","Moka Village","Morc Bagatelle","Morc Bega-Ebene","Morc Bout du Monde Ebene",
                    "Morc Eureka","Morc Hein","Morc Mon Desert","Morc Mon Plaisir","Morc Noel","Morc Parathan","Mount Ory","Pont Souillac","Railway Square","Residence Cybervillage","Telfair","Valory","Vinson","Bagatelle Mall","Martindale","Mauritius Examination Syndicate","Reduit","State House",
                    "University of Mauritius","Allées des Helvetia 3","Allées des Helvetia 2","Allées des Helvetia 1","Reduit Triangle","Cite Saint Joseph","Montagne Blanche","Morc Sans Souci","NHDC Montagne Blanche","Petit Paquet","Sans Souci","Betuel","Bonne Veine","Cite Bonne Veine","Cite Padco (A.Ravaton)",
                    "New Road","Quartier Militaire","Vuillemin","Nouvelle Decouverte","Camp Auguste","Circonstance","Cite Cote D'Or","Cote d'Or VRS 1","Helvetia","Industrial Zone-Old Post Office","Kendra","L'Agrement","Mont Fleuri","Morc Frangipanne","Morc Azalea","Morc MDA Circonstance","Morc Merrytown",
                    "Morc Raffray","Morc Traffic Centre","Morc VRS Saint Pierre","Petit Verger","Roselyn Cottage","Rte Bois Cheri","Saint Pierre","Sites and Services","Traffic Commercial Centre","Vignol NHDC","Morc Mont Desert Alma L'Agrement","Cite SILWF L'Agrement","Cite St Catherine L'Agrement",
                    "Cite EDC Cirsconstance","Tamil Temple L'Agrement","Chantenay","Vivea Business Park","Dubreuil","NHDC Dubreuil","Rishi Dayanand","Samlo","Cite Providence","Providence","Providencia 1","Providencia 2","Providencia 3","Alma","Cite EDC Verdun","Verdun","Ripailles","Allee Tamarin",
                    "Bambous","Eaux Bonnes","Geoffroy","La Ferme","Mangues Vert Doux","Mon Repos","Morcellement Eaux Bonnes","Vaudagne","Beau Songes","Camp Creoles","Cascavelle","Xavier","Carreau Eucalyptus","Case Noyale","Cite CHA","Ilot Frontier","Morcellement Multipliants","Petite Riviere Noire",
                    "Camp Madras","Chamarel","La Vieille Cheminee","La Ville Leon","Les Rouleaux","Piton","Societe St Denis","Sainte Anne","Terre Sept Couleurs","Carriere","Hilton Mauritius Resort","La Pirogue","Manguiers","Morcellement Anna 1","Morcellement Anna 2","Morcellement Bismic","Morcellement de Chazal",
                    "Morcellement Safeland 1","Morcellement Safeland 2","Morcellement Safeland 3","Morcellement Palmyre","Residence St Jacques","The Sands Resort","Sofitel Imperial","Souffleurs","Taj and Mahadhiva Hotel","Wolmar","Cite Carre dAs","Cite EDC","Cite Tamarinier","La Balise","La Balise Marina","La Preneuse",
                    "Les Salines Morcellement Plateau","London Complex","Martello Complex","Matala Villa","Morcellement Carlos","Morcellement Majo","Morcellement Mont Calme","Morcellement Paul Hein","Morcellement Diocese","Morcellement Villaze","Nautica Complex","Plantation Marguery","Riverview Commercial Centre","Ruisseau Creole Complex",
                    "West Island Villa","Croisee Chebel","Gros Cailloux","Morcellement Domah","Canot","Saint Martin","Camp des Embrevades","Canon Casse","Cite Betel","Barachois","Cap Dal","Cite EDC","Cite Padco","Domaine Mont Calme","La Mivoie","La Tourelle Ocean View","Morcellement Black Rock","Morcellement Mont Calme","Morcellement Carlos",
                    "Morcellement Corail","Morcellement Investors","Morcellement le Domain","Morcellement les Alizees","Morcellement les Salines","Morcellement Salt Pan","Morcellement la Falaise","Morcellement St Benoit","Morcellement Verojan","NHDC","Tamarina Golf","Albion","Morcellement Richwood","La Croisette","Morcellement Beerjeeraz","Morcellement de Chazal",
                    "Morcellement Raffray","Morcellement Terre dAlbion","Splendid View","Splendid Village","Coteau Raffin","La Gaulette","Morcellement Desveaux","Morcellement La Fleche","Dilo Pourie","Le Morne Brabant","Le Morne Village","La Prairie","Morcellement Gambien","Allee Tamarin","Cite Richelieu","Platform","Cite Flamboyant"};
                LookVal.lst.AddRange(Locality);
                LookVall.Add(LookVal);
                //District seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "District";
                string[] District = new string[] { "Flacq", "Grand Port", "Moka", "Pamplemousses", "Plaine Wilhems", "Port Louis", "Riviere du Rempart", "Black River", "Savnne" };
                LookVal.lst.AddRange(District);
                LookVall.Add(LookVal);
                //License Categort seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "License Category";
                string[] LicenseCategory = new string[] { "Private Car" };
                LookVal.lst.AddRange(LicenseCategory);
                LookVall.Add(LookVal);
                //Priority seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Priority";
                string[] Priority = new string[] { "Urgent", "High", "Medium", "Low" };
                LookVal.lst.AddRange(Priority);
                LookVall.Add(LookVal);
                //Vehicle Catgory seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Vehicle Category";
                string[] AssetCategory = new string[] {"DefaultCategory", "OLV", "FRT" };
                LookVal.lst.AddRange(AssetCategory);
                LookVall.Add(LookVal);
                //Vehicle Abbreviation seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Vehicle Abbreviation";
                string[] AssetSubCategory = new string[] { "ALP", "BAV", "BUS", "CL" };
                LookVal.lst.AddRange(AssetSubCategory);
                LookVall.Add(LookVal);
                //Equipment Category seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Equipment Category";
                string[] EquipmentCategory = new string[] { "EQU" };
                LookVal.lst.AddRange(EquipmentCategory);
                LookVall.Add(LookVal);
                //Equipment Abbreviation seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Equipment Abbreviation";
                string[] EquipmentSubCategory = new string[] { "AK", "AED", "AFL", "AFF" };
                LookVal.lst.AddRange(EquipmentSubCategory);
                LookVall.Add(LookVal);
                //Casualty Type seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Casualty Type";
                string[] CasualtyType = new string[] { "Burn" };
                LookVal.lst.AddRange(CasualtyType);
                LookVall.Add(LookVal);
                //Fatality Type seeding
                LookVal = new ValueSeeding();
                LookVal.Name = "Fatality Type";
                string[] Fatality = new string[] { "Death" };
                LookVal.lst.AddRange(Fatality);
                LookVall.Add(LookVal);

                foreach (string LookUp in LookUpList)
                {
                    NPF_SYS_LookUpType ltc = context.NPF_SYS_LookUpType.Where(x => x.Name == LookUp).FirstOrDefault();
                    if (ltc == null)
                    {

                        ltc = new NPF_SYS_LookUpType()
                        {
                            Name = LookUp,
                            Description = LookUp,
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.Now

                        };
                        context.NPF_SYS_LookUpType.Add(ltc);
                        context.SaveChanges();
                    }

                    List<NPF_SYS_LookUpValue> lvl = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Id == ltc.Id).ToList();
                    var slvl = LookVall.Where(x => x.Name == LookUp).FirstOrDefault();
                    if (slvl.lst.Count > 0)
                    {
                        foreach (var vs in slvl.lst)
                        {
                            if (!lvl.Any(x => x.Name == vs))
                            {
                                NPF_SYS_LookUpValue lv = new NPF_SYS_LookUpValue()
                                {
                                    Name = vs,
                                    LookUpType = ltc,
                                    Description = vs,
                                    CreatedBy = createdBy,
                                    CreatedDate = DateTime.Now
                                };
                                context.NPF_SYS_LookUpValue.Add(lv);
                                context.SaveChanges();
                            }
                        }
                    }


                }

            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedModules(UserManager<ApplicationUser> userManager)
        {
            NPFContext context = new NPFContext();
            var user = userManager.FindByNameAsync("admin@muzztech.mu").Result;

            #region Dashboard
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Dashboard"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Dashboard",
                    Url = "Incident/List",
                    Order = 0,
                    Icon = "fa fa-dashboard",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Call Handling
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Call handling"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Call handling",
                    Url = "IncidentHandling/List",
                    Order = 0,
                    Icon = "fa fa-dashboard",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion


            #region Adminstration and sub menus

            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Administration"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Administration",
                    Url = "",
                    Order = 2,
                    Icon = "fa fa-gears",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }

            if (!context.NPF_SYS_Modules.Any(x => x.Name == "General"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "General",
                    Url = "",
                    Order = 3,
                    Icon = "fa fa-tools",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Administration").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "LookUp Value"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "LookUp Value",
                    Url = "LookUpValue/List",
                    Order = 4,
                    Icon = "fa fa-gear",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Name and Address"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Name and Address",
                    Url = "NAA/List",
                    Order = 6,
                    Icon = "fa fa-user",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Station"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Station",
                    Url = "Station/List",
                    Order = 14,
                    Icon = "fa fa-users-gear",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Global Param"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Global Param",
                    Url = "GlobalParam/List",
                    Order = 20,
                    Icon = "fa-solid fa-network-wired",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Zone Type"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Zone Type",
                    Url = "ZoneType/List",
                    Order = 21,
                    Icon = "fa fa-draw-polygon",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Zone"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Zone",
                    Url = "Zone/List",
                    Order = 22,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "External Agency"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "External Agency",
                    Url = "ExternalAgencies/List",
                    Order = 23,
                    Icon = "fa fa-gear",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Incident Type"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Incident Type",
                    Url = "IncidentType/List",
                    Order = 25,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Station Type"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Station Type",
                    Url = "StationType/List",
                    Order = 24,
                    Icon = "fa fa-draw-polygon",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Incident Sub Type"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Incident Sub Type",
                    Url = "IncidentSubType/List",
                    Order = 26,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Nature of Incident"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Nature of Incident",
                    Url = "NatureofIncident/List",
                    Order = 27,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Incident Code"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Incident Code",
                    Url = "IncidentCode/List",
                    Order = 28,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Vehicle Make"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Vehicle Make",
                    Url = "AssetMake/List",
                    Order = 29,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Rank"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Rank",
                    Url = "Rank/List",
                    Order = 31,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Vehicle Model"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Vehicle Model",
                    Url = "AssetModel/List",
                    Order = 30,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Shift"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Shift",
                    Url = "Shift/List",
                    Order = 27,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Roster"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Roster",
                    Url = "Roster/List",
                    Order = 27,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Pre-Determined Attandance"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Pre-Determined Attandance",
                    Url = "PDA/List",
                    Order = 28,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Resource" && x.ParentId == context.NPF_SYS_Modules.Where(x => x.Name == "Administration").Select(x => x.Id).FirstOrDefault()))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Resource",
                    Url = "",
                    Order = 15,
                    Icon = "fa fa-tools",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Administration").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Resource Management" && x.ParentId == context.NPF_SYS_Modules.Where(x => x.Name == "Resource").Select(x => x.Id).FirstOrDefault()))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Resource Management",
                    Url = "Resource/List",
                    Order = 13,
                    Icon = "fa fa-users-gear",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Resource").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Resource Category"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Resource Category",
                    Url = "ResourceType/List",
                    Order = 12,
                    Icon = "fa-solid fa-book",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Resource").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Caller"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Caller",
                    Url = "Caller/List",
                    Order = 34,
                    Icon = "fa fa-users-gear",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Resource").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Asset Management"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Asset Management",
                    Url = "",
                    Order = 3,
                    Icon = "fa fa-tools",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Administration").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Equipment"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Equipment",
                    Url = "Equipment/List",
                    Order = 33,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Asset Management").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Vehicle"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Vehicle",
                    Url = "Vehicle/List",
                    Order = 32,
                    Icon = "fa fa-car",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Asset Management").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Equipment Make"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Equipment Make",
                    Url = "EquipmentMake/List",
                    Order = 35,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Equipment Model"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Equipment Model",
                    Url = "EquipmentModel/List",
                    Order = 36,
                    Icon = "fa fa-pen-ruler",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Priority"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()

                {
                    Name = "Priority",
                    Url = "Priority/List",
                    Order = 36,
                    Icon = "fa fa-triangle-exclamation",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "General").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            #endregion

            #region Access
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Access"))
            {
                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Access",
                    Url = "",
                    Order = 21,
                    Icon = "fa fa-users-gear",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Administration").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "User"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "User",
                    Url = "Users/List",
                    Order = 22,
                    Icon = "fa fa-user",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Access Rights"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Access Rights",
                    Url = "AccessRights/List",
                    Order = 23,
                    Icon = "fa fa-circle-user",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Matrix"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Matrix",
                    Url = "Matrix/Matrix",
                    Order = 24,
                    Icon = "fa fa-folder-tree",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Menu"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Menu",
                    Url = "Menu/List",
                    Order = 24,
                    Icon = "fa-regular fa-square-caret-down",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "Office Zones"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "Office Zones",
                    Url = "OfficeZones/List",
                    Order = 24,
                    Icon = "fa-regular fa-square-caret-down",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            if (!context.NPF_SYS_Modules.Any(x => x.Name == "User Type"))
            {

                context.NPF_SYS_Modules.Add(new NPF_SYS_Modules()
                {
                    Name = "User Type",
                    Url = "UserType/List",
                    Order = 25,
                    Icon = "fa fa-layer-group",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }

            #endregion Access

            List<NPF_SYS_Modules> dtl = context.NPF_SYS_Modules.Where(x => x.DisplayName == null).ToList();
            if (dtl.Count > 0)
            {
                foreach (NPF_SYS_Modules dt in dtl)
                {
                    dt.DisplayName = dt.Name;
                    context.NPF_SYS_Modules.Update(dt);
                    context.SaveChanges();
                }

            }
        }
        public static void SeedAcessRights(RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                if (!roleManager.RoleExistsAsync("Administrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "Administrator";
                    role.NormalizedName = "ADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
                ApplicationRole AdmRole = context.Roles.Where(x => x.Name == "Administrator").FirstOrDefault();
                context.Roles.Update(AdmRole);
                List<NPF_SYS_Modules> mdl = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false).ToList();
                if (mdl.Count > 0)
                {
                    foreach (NPF_SYS_Modules md in mdl)
                    {
                        if (!context.NPF_SYS_AccessRights.Any(x => x.Role.Id == AdmRole.Id && x.Module.Id == md.Id))
                        {
                            context.NPF_SYS_Modules.Update(md);
                            NPF_SYS_AccessRights ar = new NPF_SYS_AccessRights()
                            {
                                hasRight = true,
                                Role = AdmRole,
                                Module = md,
                                CreatedBy = createdBy,
                                CreatedDate = DateTime.Now,
                                DeleteStatus = false

                            };
                            context.NPF_SYS_AccessRights.Add(ar);
                            context.SaveChanges();

                        }

                    }
                }


            }
            catch (Exception e)
            {

            }

        }
        public static void SeedAcessRightsN(RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                if (!roleManager.RoleExistsAsync("SuperAdministrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "SuperAdministrator";
                    role.NormalizedName = "SUPERADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }

                ApplicationRole AdmRole = context.Roles.Where(x => x.Name == "SuperAdministrator").FirstOrDefault();
                context.Roles.Update(AdmRole);
                List<NPF_SYS_Modules> mdl = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false).ToList();
                if (mdl.Count > 0)
                {
                    foreach (NPF_SYS_Modules md in mdl)
                    {
                        if (!context.NPF_SYS_AccessRights.Any(x => x.Role.Id == AdmRole.Id && x.Module.Id == md.Id))
                        {
                            try
                            {
                                context.NPF_SYS_Modules.Update(md);
                                NPF_SYS_AccessRights ar = new NPF_SYS_AccessRights()
                                {
                                    hasRight = true,
                                    Role = AdmRole,
                                    Module = md,
                                    CreatedBy = createdBy,
                                    CreatedDate = DateTime.Now,
                                    DeleteStatus = false

                                };
                                context.NPF_SYS_AccessRights.Add(ar);
                                context.SaveChanges();

                                foreach (AccessOperationType g in Enum.GetValues(typeof(AccessOperationType)))
                                {
                                    NPF_SYS_AccessRightsDetails ARD = new NPF_SYS_AccessRightsDetails()
                                    {
                                        AccessRights = ar,
                                        AOT = g,
                                        Permission = true
                                    };
                                    context.NPF_SYS_AccessRightsDetails.Add(ARD);
                                    context.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
                List<NPF_SYS_AccessRights> ACCL = context.NPF_SYS_AccessRights.Where(x => x.DeleteStatus == false).ToList();
                if (ACCL.Count > 0)
                {
                    foreach (NPF_SYS_AccessRights ACC in ACCL)
                    {
                        if (!context.NPF_SYS_AccessRightsDetails.Any(x => x.AccessRights.id == ACC.id && x.DeleteStatus == false && x.AOT == AccessOperationType.Export))
                        {
                            NPF_SYS_AccessRightsDetails ARD = new NPF_SYS_AccessRightsDetails()
                            {
                                AccessRights = ACC,
                                AOT = AccessOperationType.Export,
                                Permission = false
                            };
                            context.NPF_SYS_AccessRightsDetails.Add(ARD);
                            context.SaveChanges();
                        }
                    }
                }


            }
            catch (Exception e)
            {

            }

        }
        public static async Task SeedTableCodeConfigurations(UserManager<ApplicationUser> userManager)
        {
            try
            {

                NPFContext context = new NPFContext();
                List<NPF_SYS_TableCodeConfigurations> LCconf = context.NPF_SYS_TableCodeConfigurations.Where(x => x.HasAddi == null).ToList();
                foreach (NPF_SYS_TableCodeConfigurations Cconf in LCconf)
                {
                    Cconf.HasAddi = false;
                    context.NPF_SYS_TableCodeConfigurations.Update(Cconf);
                    context.SaveChanges();

                }
                string[] TblName = new string[] {
                    "NPF_SYS_Station-STN","NPF_DIR_NAA-NAA","NPF_SYS_Resource-RES","NPF_SYS_Department-DPT", "NPF_SYS_Division-DIV",
                    "NPF_FRM_Form-FRM", "NPF_FRM_FormType-FMT", "NPF_FRM_Section-SCT", "NPF_FRM_FormPage-FPG","NPF_SYS_OfficeZones-OFZ",
                    "NPF_STR_Store-STR", "NPF_STR_StoreItem-STI", "NPF_STR_StoreTransaction-STT", "NPF_SYS_WorkFlowHeader-WFH",
                    "NPF_SYS_WorkFlowDetails-WFD", "NPF_SCH_Schedule-SCH", "NPF_SYS_ZoneType-ZNT", "NPF_SYS_Zone-ZON",
                    "NPF_SYS_ExternalAgencies-EXT", "NPF_SYS_IncidentType-INT", "NPF_SYS_StationType-STT", "NPF_SYS_IncidentSubType-IST",
                "NPF_SYS_UserType-UTT", "NPF_SYS_NatureOfIncident-NOI", "NPF_SYS_IncidentCode-ICC", "NPF_SYS_AssetMake-MAK", "NPF_SYS_AssetModel-MOD",
                "NPF_SYS_Rank-RNK","NPF_SYS_Shift-SHT", "NPF_SYS_Vehicle-VEH", "NPF_SYS_Equipment-EQP", "NPF_TRN_Incident-INC","NPF_SYS_Roster-ROS",
                    "NPF_SYS_PDA-PDA","NPF_TRN_IncidentNumber-FC", "NPF_SYS_Priority-PRT"
                };
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                NPF_SYS_CodeConfiguration cc = context.NPF_SYS_CodeConfiguration.Where(x => x.Name == "Default").FirstOrDefault();
                if (cc == null)
                {
                    cc = new NPF_SYS_CodeConfiguration()
                    {
                        Name = "Default",
                        Date = false,
                        Month = false,
                        Year = false,
                        UsePrefix = true,
                        PaddingNo = 0,
                        DateFormat = string.Empty,
                        YearFormat = string.Empty,
                        MonthFormat = string.Empty,
                        ResetConfig = string.Empty,
                        Comment = "Default if no config Found",
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now
                    };
                    context.NPF_SYS_CodeConfiguration.Add(cc);
                    context.SaveChanges();
                }

                cc = context.NPF_SYS_CodeConfiguration.Where(x => x.Name == "IncidentNumber").FirstOrDefault();
                if (cc == null)
                {
                    cc = new NPF_SYS_CodeConfiguration()
                    {
                        Name = "IncidentNumber",
                        Date = false,
                        Month = false,
                        Year = true,
                        UsePrefix = true,
                        PaddingNo = 0,
                        DateFormat = string.Empty,
                        YearFormat = "yyyy",
                        MonthFormat = string.Empty,
                        ResetConfig = "Year",
                        Comment = "Default if no config Found",
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now,
                        Reset = true,
                    };
                    context.NPF_SYS_CodeConfiguration.Add(cc);
                    context.SaveChanges();
                }

                NPF_SYS_CompanyList cl = context.NPF_SYS_CompanyList.Where(x => x.Name == "Default").FirstOrDefault();
                if (cl == null)
                {
                    cl = new NPF_SYS_CompanyList()
                    {
                        ParentId = 1,
                        Name = "Default",
                        Value = "",
                        AdditionalValue = "",
                        Enable = true,
                        Comment = "Default if no Company Found",
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_SYS_CompanyList.Add(cl);
                    context.SaveChanges();
                }
                foreach (string name in TblName)
                {
                    string[] splt = name.Split('-');
                    NPF_SYS_TableCodeConfigurations tcc = context.NPF_SYS_TableCodeConfigurations.Where(x => x.TableName == splt[0] && x.CompanyId == cl.Id).FirstOrDefault();
                    if (tcc == null)
                    {
                        tcc = new NPF_SYS_TableCodeConfigurations()
                        {
                            TableName = splt[0],
                            Prefix = splt[1],
                            CompanyId = cl.Id,
                            ConfigurationId = cc.Id,
                            Comment = "Default Table Config",
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.Now,
                            HasAddi = true
                        };
                        context.NPF_SYS_TableCodeConfigurations.Add(tcc);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedActivityEventType(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());

                List<string> SchemeList = new List<string> { "Approval" };
                foreach (string item in SchemeList)
                {

                    if (!context.NPF_ACT_ActivityEventType.Any(x => x.Name == "Approval" && x.DeleteStatus == false))
                    {
                        NPF_ACT_ActivityEventType AC = new NPF_ACT_ActivityEventType();
                        AC.Name = "Approval";
                        AC.Type = context.NPF_SYS_LookUpValue.Where(x => x.Name == "General" && x.LookUpType.Name == "ActivityTypeMappingList").FirstOrDefault();
                        AC.IsEvent = false;
                        AC.IsActivity = true;
                        AC.TriggerActivity = false;
                        AC.CreatedBy = createdBy;
                        AC.CreatedDate = DateTime.Now;
                        context.NPF_ACT_ActivityEventType.Add(AC);
                        context.SaveChanges();
                    };

                }
            }
            catch (Exception e)
            {

            }
        }
        public static async void SeedGlobalParam(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "UniqueCodeGenerator"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "UniqueCodeGenerator";
                    GP.Value = "0";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "CashReceipt"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "CashReceipt";
                    GP.Value = "http://localhost:53026/NPF/MouraCashReceipt";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }

                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "ApplicationId"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "ApplicationId";
                    GP.AdminOnly = true;
                    GP.Value = "AAAA5g1iTvI:APA91bGqcQDidmMvm4hey6ib8v1CHC1gBxGrm-uyP75EeqyfqAcpyfpi0eyyWx-5CrzSJI7T-Qc6QsCarVePdSXPmTnEB5_leFu_A7y6cFYeh_FJnn7fpa7Kum20Th1EOpQyAIdTDtQ1";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "MaintenanceConn"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "MaintenanceConn";
                    GP.AdminOnly = true;
                    GP.Value = "<add name=\"DB99\" connectionString=\"Data Source=******;Initial Catalog=****;User ID=****;Password=*****;Integrated Security=False;\" providerName=\"System.Data.SqlClient\" />";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }

                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "senderId"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "senderId";
                    GP.AdminOnly = true;
                    GP.Value = "988067024626";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }

                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "DefaultPassword"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "DefaultPassword";
                    GP.AdminOnly = true;
                    GP.Value = "muzztech@2022.";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "ExternalURL"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "ExternalURL";
                    GP.AdminOnly = true;
                    GP.Value = "https://*****.muzztech.mu";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "BasePath"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "BasePath";
                    GP.Value = "C:\\FareiFiles";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "ApplicationURL"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "ApplicationURL";
                    GP.Value = "https://*****.muzztech.mu";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FormPageNumber"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FormPageNumber";
                    GP.Value = "0";
                    GP.AdditionalValue = DateTime.Now.Year.ToString();
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FleetDataURL"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FleetDataURL";
                    GP.Value = "https://mfrsfems.govmu.org/api/v2.0/";
                    GP.Comment = "Fleet Data URL";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FleetLoginPayload"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FleetLoginPayload";
                    GP.Value = "{\"user\":{\"email\":\"mfrsfems@muzztech.mu\",\"password\":\"V+I0GFr9cFGH/FPTxLs80o9ux/78MNQW4e3Mh58c4chNh3bnxe1QNnokWaV9VIZLJfpXK3OFqXRRrJHJzL1MM5wNnQwoFe0OE/sFRsHSSsPrEvU2927KEmqYcFok3w//cT4Nr1WPc17bSD/+XzvnGfBQG9uKj4E+u7KSr0Ep6+I=\"},\"companyToken\":\"hA2.*c9x6a2PgaZMi_6Sg328@1pc35p9\"}";
                    GP.Comment = "Fleet Login Payload";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FleetMapPayload"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FleetMapPayload";
                    GP.Value = "{\"assetIds\":[106,99,101,97,126,131,21,110,60,124,119,13,22,111,125,98,69,20,18,26,62,25,24,28,107,11,17,123,23,33,72,70,31,16,30,27,103,29,35,1,66,34,3,109,6,9,10,67,4,8,19,68,7,32,5,12,104,36,95,92,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,186,187,188,132,77,76,122,85,57,134,115,78,75,133,138,128,74,59,91,56,61,96,86,73,102,81,82,80,79,83,84,129,55,93,152],\"byDriver\":false}";
                    GP.Comment = "Fleet Data Payload";
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "IncidentNo"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "IncidentNo";
                    GP.Value = "0";
                    GP.AdditionalValue = "2024";
                    GP.Comment = "Current Incident No for the Year and the Prefix";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "IncidentPrefix"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "IncidentPrefix";
                    GP.Value = "FC";
                    GP.Comment = "Prefix";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "IsUAT"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "IsUAT";
                    GP.Value = "1";
                    GP.Comment = "If 1 then true : 0";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "BackgroundServiceTimer"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "BackgroundServiceTimer";
                    GP.Value = "300";
                    GP.Comment = "Param in seconds";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "RunBackgroundService"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "RunBackgroundService";
                    GP.Value = "0";
                    GP.Comment = "If 1 then true : 0";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FUserToken"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FUserToken";
                    GP.Value = "userToken";
                    GP.Comment = "Fleet UserToken";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FEmail"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FEmail";
                    GP.Value = "FEmail";
                    GP.Comment = "Fleet Email";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FPassword"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FPassword";
                    GP.Value = "FPassword";
                    GP.Comment = "Fleet Password";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }

                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FcompanyToken"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FcompanyToken";
                    GP.Value = "hA2.*c9x6a2PgaZMi_6Sg328@1pc35p9";
                    GP.Comment = "Fleet companyToken";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FLogin"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FLogin";
                    GP.Value = "Login";
                    GP.Comment = "Fleet Login";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FGetMasterData"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FGetMasterData";
                    GP.Value = "GetMasterData";
                    GP.Comment = "Fleet GetMasterData";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FLive"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FLive";
                    GP.Value = "Live";
                    GP.Comment = "Fleet Live";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FEquipment"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "FEquipment";
                    GP.Value = "GetAllEquipmentList";
                    GP.Comment = "Fleet Equipment";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "AlertTimer"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "AlertTimer";
                    GP.Value = "1";
                    GP.Comment = "In Minutes";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "BatchNotification"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "BatchNotification";
                    GP.Value = "1";
                    GP.Comment = "BachID";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "NotificationRows"))
                {
                    NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                    GP.Name = "NotificationRows";
                    GP.Value = "10";
                    GP.Comment = "Top Rows";
                    GP.AdminOnly = true;
                    context.NPF_SYS_GlobalParam.Add(GP);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedUOM(UserManager<ApplicationUser> userManager)
        {
            try
            {

                NPFContext context = new NPFContext();
                Guid createdBy = new Guid();
                if (context.Users.Any(x => x.Email.ToLower() == "admin@muzztech.mu"))
                {
                    createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                }
                //Add Name of UOM Header to be seeded in list below
                string[] List = new string[] { "Land", "Weight", "Crop" };
                List<ValueSeeding> Vall = new List<ValueSeeding>();
                ValueSeeding Val = new ValueSeeding();

                //Duplicate within comments to add UOMDetails Seeding
                Val = new ValueSeeding();
                Val.Name = "Land";//Replace with UOM Header Name
                string[] Land = new string[] { "m²-m²-1-1.0", "ha-Hectare-1-1.06", "Arp-Arpent-1-4046.86" };//"Name-Unit-IssystemInternational-RelativeFactor"
                Val.lst.AddRange(Land);
                Vall.Add(Val);

                //Duplicate within comments to add UOMDetails Seeding
                Val = new ValueSeeding();
                Val.Name = "Weight";//Replace with UOM Header Name
                string[] Weight = new string[] { "g-gram-1-1.0", "kg-kilogram-1-1.06" };//"Name-Unit-IssystemInternational-RelativeFactor"
                Val.lst.AddRange(Weight);
                Vall.Add(Val);

                //Duplicate within comments to add UOMDetails Seeding
                Val = new ValueSeeding();
                Val.Name = "Crop";//Replace with UOM Header Name
                string[] Crop = new string[] { "Plants-Plants-1-1.0" };//"Name-Unit-IssystemInternational-RelativeFactor"
                Val.lst.AddRange(Crop);
                Vall.Add(Val);

                foreach (string Header in List)
                {
                    NPF_SYS_UomHeader ltc = context.NPF_SYS_UomHeader.Where(x => x.Name == Header).FirstOrDefault();
                    if (ltc == null)
                    {
                        ltc = new NPF_SYS_UomHeader()
                        {
                            Name = Header,
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.Now

                        };
                        context.NPF_SYS_UomHeader.Add(ltc);
                        context.SaveChanges();
                    }

                    List<NPF_SYS_UomDetails> lvl = context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Id == ltc.Id).ToList();
                    var slvl = Vall.Where(x => x.Name == Header).FirstOrDefault();
                    context.NPF_SYS_UomHeader.Update(ltc);
                    if (slvl.lst.Count > 0)
                    {
                        foreach (var vs in slvl.lst)
                        {
                            string[] splt = vs.Split('-');
                            if (!lvl.Any(x => x.Name == splt[0]))
                            {
                                NPF_SYS_UomDetails lv = new NPF_SYS_UomDetails()
                                {
                                    Name = splt[0],
                                    UomHeader = ltc,
                                    Unit = splt[1],
                                    IsSystemInternational = splt[2] == "1" ? true : false,
                                    RelativeFactor = float.Parse(splt[3]),
                                    CreatedBy = createdBy,
                                    CreatedDate = DateTime.Now
                                };
                                context.NPF_SYS_UomDetails.Add(lv);
                                context.SaveChanges();
                            }
                        }
                    }


                }

            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedResourceType(UserManager<ApplicationUser> userManager)
        {
            try
            {

                NPFContext context = new NPFContext();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").FirstOrDefault();
                if (!context.NPF_SYS_ResourceType.Any(x => x.Name == "Officer"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_ResourceType");
                    NPF_SYS_ResourceType RT = new NPF_SYS_ResourceType();
                    RT.Name = "Officer";
                    RT.IsSystem = false;
                    RT.Description = "Officer";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_ResourceType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_ResourceType.Any(x => x.Name == "CRO"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_ResourceType");
                    NPF_SYS_ResourceType RT = new NPF_SYS_ResourceType();
                    RT.Name = "CRO";
                    RT.IsSystem = true;
                    RT.Description = "CRO";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_ResourceType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_ResourceType.Any(x => x.Name == "Station"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_ResourceType");
                    NPF_SYS_ResourceType RT = new NPF_SYS_ResourceType();
                    RT.Name = "Station";
                    RT.IsSystem = true;
                    RT.Description = "Station";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_ResourceType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_ResourceType.Any(x => x.Name == "Station Commander"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_ResourceType");
                    NPF_SYS_ResourceType RT = new NPF_SYS_ResourceType();
                    RT.Name = "Station Commander";
                    RT.IsSystem = true;
                    RT.Description = "Station Commander";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_ResourceType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_ResourceType.Any(x => x.Name == "Crew Commander"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_ResourceType");
                    NPF_SYS_ResourceType RT = new NPF_SYS_ResourceType();
                    RT.Name = "Crew Commander";
                    RT.IsSystem = true;
                    RT.Description = "Crew Commander";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_ResourceType.Add(RT);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedStationType(UserManager<ApplicationUser> userManager)
        {
            try
            {

                NPFContext context = new NPFContext();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").FirstOrDefault();
                if (!context.NPF_SYS_StationType.Any(x => x.Name == "Main Fire Station"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_StationType");
                    NPF_SYS_StationType RT = new NPF_SYS_StationType();
                    RT.Name = "Main Fire Station";
                    RT.Description = "Main Fire Station";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_StationType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_StationType.Any(x => x.Name == "Sub Fire Station"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_StationType");
                    NPF_SYS_StationType RT = new NPF_SYS_StationType();
                    RT.Name = "Sub Fire Station";
                    RT.Description = "Sub Fire Station";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_StationType.Add(RT);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedAssetMakeModel(UserManager<ApplicationUser> userManager)
        {
            try
            {

                NPFContext context = new NPFContext();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").FirstOrDefault();
                if (!context.NPF_SYS_AssetMake.Any(x => x.Name == "DefaultMake"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_AssetMake");
                    NPF_SYS_AssetMake dt = new NPF_SYS_AssetMake()
                    {
                        Code = BaseDtoS.Data,
                        Name = "DefaultMake",
                        Description = "DefaultMake",
                    };
                    context.NPF_SYS_AssetMake.Add(dt);
                    context.SaveChanges();
                    if (!context.NPF_SYS_AssetModel.Any(x => x.Name == "DefaultModel"))
                    {
                        BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_AssetModel");
                        NPF_SYS_AssetModel dx = new NPF_SYS_AssetModel()
                        {
                            Code = BaseDtoS.Data,
                            Name = "DefaultModel",
                            Description = "DefaultModel",
                            AssetMake = context.NPF_SYS_AssetMake.Where(x => x.Id == dt.Id).FirstOrDefault(),
                        };
                        context.NPF_SYS_AssetModel.Add(dx);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {

            }

        }

        public static async void SeedIncidentType(UserManager<ApplicationUser> userManager)
        {
            try
            {

                NPFContext context = new NPFContext();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").FirstOrDefault();
                if (!context.NPF_SYS_IncidentType.Any(x => x.Name == "Fire Incident"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_IncidentType");
                    NPF_SYS_IncidentType RT = new NPF_SYS_IncidentType();
                    RT.Name = "Fire Incident";
                    RT.Description = "Fire Incident";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_IncidentType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_IncidentType.Any(x => x.Name == "Non Fire Incident"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_IncidentType");
                    NPF_SYS_IncidentType RT = new NPF_SYS_IncidentType();
                    RT.Name = "Non Fire Incident";
                    RT.Description = "Non Fire Incident";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_IncidentType.Add(RT);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_IncidentType.Any(x => x.Name == "Non Incident"))
                {
                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(Usr.UserToken.ToString(), "NPF_SYS_IncidentType");
                    NPF_SYS_IncidentType RT = new NPF_SYS_IncidentType();
                    RT.Name = "Non Incident";
                    RT.Description = "Non Incident";
                    RT.Code = BaseDtoS.Data;
                    RT.CreatedBy = Guid.Parse(Usr.Id);
                    RT.CreatedDate = DateTime.Now;
                    context.NPF_SYS_IncidentType.Add(RT);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedGroupMatrix(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                NPF_SYS_GroupMatrix RGM = new NPF_SYS_GroupMatrix();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "superadmin@muzztech.mu").FirstOrDefault();
                var SysUsr = context.NPF_SYS_User.Where(x => x.AId == Usr.Id).FirstOrDefault();
                if (!context.NPF_SYS_GroupMatrix.Any(x => x.GMDescription == "ROOT"))
                {
                    RGM = new NPF_SYS_GroupMatrix();
                    RGM.ParentGMID = null;
                    RGM.GMDescription = "ROOT";
                    RGM.Remarks = "ROOT";
                    RGM.CompanyCode = "COMP1";
                    RGM.IsCompany = false;
                    RGM.LegalName = "ROOT";
                    RGM.CreatedBy = Guid.Parse(Usr.Id);
                    RGM.CreatedDate = DateTime.Now;
                    context.NPF_SYS_GroupMatrix.Add(RGM);
                    context.SaveChanges();
                }
                List<NPF_SYS_GroupMatrix> mlt = context.NPF_SYS_GroupMatrix.Where(x => x.ParentGMID == null && x.GMID != RGM.GMID).ToList();
                foreach (NPF_SYS_GroupMatrix m in mlt)
                {
                    m.ParentGMID = RGM.GMID;
                    context.NPF_SYS_GroupMatrix.Update(m);
                    context.SaveChanges();
                }
                if (!context.NPF_SYS_GroupMatrix.Any(x => x.GMDescription == "MFRS"))
                {
                    NPF_SYS_GroupMatrix GM = new NPF_SYS_GroupMatrix();
                    GM.ParentGMID = RGM.GMID;
                    GM.GMDescription = "MFRS";
                    GM.Remarks = "MFRS";
                    GM.CompanyCode = "COMP1";
                    GM.IsCompany = false;
                    GM.LegalName = "MFRS";
                    GM.CreatedBy = Guid.Parse(Usr.Id);
                    GM.CreatedDate = DateTime.Now;
                    context.NPF_SYS_GroupMatrix.Add(GM);
                    context.SaveChanges();
                }

                if (!context.NPF_SYS_GroupMatrixUser.Any(x => x.IID == SysUsr.Id))
                {
                    NPF_SYS_GroupMatrixUser GMU = new NPF_SYS_GroupMatrixUser();
                    GMU.IID = SysUsr.Id;
                    GMU.GMID = RGM.GMID;
                    GMU.CreatedBy = Guid.Parse(Usr.Id);
                    GMU.CreatedDate = DateTime.Now;
                    context.NPF_SYS_GroupMatrixUser.Add(GMU);
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedInputType(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                List<NPF_FRM_InputType> Intl = new List<NPF_FRM_InputType>();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                string[] InputTypeList = new string[] { "Text", "Time", "Number", "Checkbox", "Radio", "Dropdown", "Datatable", "Date", "Label", "Multiline", "Datetime", "Signature", "Attachment" };
                List<ValueSeeding> InputDetailVall = new List<ValueSeeding>();
                ValueSeeding InputDetailVal = new ValueSeeding();

                //Duplicate within comments to add InputTypeDetails Seeding
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Text";//Replace with InputType Name
                string[] Text = new string[] { "Text", "Read-Only Text" };// Build List Of values
                InputDetailVal.lst.AddRange(Text);
                InputDetailVall.Add(InputDetailVal);
                //Duplicate within comments to add InputTypeDetails Seeding

                //Time
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Time";//Replace with InputType Name
                string[] Time = new string[] { "Time" };// Build List Of values
                InputDetailVal.lst.AddRange(Time);
                InputDetailVall.Add(InputDetailVal);
                //Number
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Number";//Replace with InputType Name
                string[] Number = new string[] { "Number", "Number With UOM" };// Build List Of values
                InputDetailVal.lst.AddRange(Number);
                InputDetailVall.Add(InputDetailVal);
                //Checkbox
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Checkbox";//Replace with InputType Name
                string[] Checkbox = new string[] { "Checkbox", "Checkbox With Remarks" };// Build List Of values
                InputDetailVal.lst.AddRange(Checkbox);
                InputDetailVall.Add(InputDetailVal);
                //Radio
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Radio";//Replace with InputType Name
                string[] Radio = new string[] { "Radio" };// Build List Of values
                InputDetailVal.lst.AddRange(Radio);
                InputDetailVall.Add(InputDetailVal);
                //Dropdown
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Dropdown";//Replace with InputType Name
                string[] Dropdown = new string[] { "Dropdown", "Dropdown With Add New", "Dropdown with Details", "Dropdown With Details and Add New" };// Build List Of values
                InputDetailVal.lst.AddRange(Dropdown);
                InputDetailVall.Add(InputDetailVal);
                //Datatable
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Datatable";//Replace with InputType Name
                string[] Datatable = new string[] { "Datatable" };// Build List Of values
                InputDetailVal.lst.AddRange(Datatable);
                InputDetailVall.Add(InputDetailVal);
                //Date
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Date";//Replace with InputType Name
                string[] Date = new string[] { "Date" };// Build List Of values
                InputDetailVal.lst.AddRange(Date);
                InputDetailVall.Add(InputDetailVal);
                //Datetime
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Datetime";//Replace with InputType Name
                string[] Datetime = new string[] { "Datetime" };// Build List Of values
                InputDetailVal.lst.AddRange(Datetime);
                InputDetailVall.Add(InputDetailVal);
                //Datatable
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Multiline";//Replace with InputType Name
                string[] Multiline = new string[] { "Multiline", "Read-Only Multiline" };// Build List Of values
                InputDetailVal.lst.AddRange(Multiline);
                InputDetailVall.Add(InputDetailVal);
                //Label
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Label";//Replace with InputType Name
                string[] Label = new string[] { "Label" };// Build List Of values
                InputDetailVal.lst.AddRange(Label);
                InputDetailVall.Add(InputDetailVal);
                //Signature
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Signature";//Replace with InputType Name
                string[] Signature = new string[] { "Signature" };// Build List Of values
                InputDetailVal.lst.AddRange(Signature);
                InputDetailVall.Add(InputDetailVal);
                //Attachment
                InputDetailVal = new ValueSeeding();
                InputDetailVal.Name = "Attachment";//Replace with InputType Name
                string[] Attachment = new string[] { "Attachment" };// Build List Of values
                InputDetailVal.lst.AddRange(Attachment);
                InputDetailVall.Add(InputDetailVal);


                foreach (string InputType in InputTypeList)
                {
                    NPF_FRM_InputType Int = context.NPF_FRM_InputType.Where(x => x.Name == InputType).FirstOrDefault();
                    if (Int == null)
                    {
                        Int = new NPF_FRM_InputType()
                        {
                            Name = InputType,
                            Description = InputType,
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.Now

                        };
                        context.NPF_FRM_InputType.Add(Int);
                        context.SaveChanges();
                    }
                    List<NPF_FRM_InputTypeDetails> lvl = context.NPF_FRM_InputTypeDetails.Where(x => x.InputType.Id == Int.Id).ToList();
                    var slvl = InputDetailVall.Where(x => x.Name == InputType).FirstOrDefault();
                    if (slvl != null)
                    {
                        foreach (var vs in slvl.lst)
                        {
                            if (!lvl.Any(x => x.Name == vs))
                            {
                                context.NPF_FRM_InputType.Update(Int);
                                NPF_FRM_InputTypeDetails lv = new NPF_FRM_InputTypeDetails()
                                {
                                    Name = vs,
                                    InputType = Int,
                                    Description = vs,
                                    CreatedBy = createdBy,
                                    CreatedDate = DateTime.Now
                                };
                                context.NPF_FRM_InputTypeDetails.Add(lv);
                                context.SaveChanges();
                            }
                        }
                    }


                }


            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedSource(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                List<NPF_FRM_InputType> Intl = new List<NPF_FRM_InputType>();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());
                string[] SourceTypeList = new string[] { "Query", "Procedure", "Views" };
                List<ValueSeeding> InputDetailVall = new List<ValueSeeding>();
                ValueSeeding SourceVal = new ValueSeeding();

                //Duplicate within comments to add InputTypeDetails Seeding
                SourceVal = new ValueSeeding();
                SourceVal.Name = "Query";//Replace with InputType Name
                string[] Query = new string[] { "NPF_SYS_LookUpValue" , "NPF_DIR_NAA", "NPF_SYS_ResourceType",
                     "NPF_SYS_Station", "NPF_SYS_Resource",  "NPF_SYS_User" };// Build List Of values
                SourceVal.lst.AddRange(Query);
                InputDetailVall.Add(SourceVal);
                //Duplicate within comments to add InputTypeDetails Seeding

                foreach (string Type in SourceTypeList)
                {
                    NPF_FRM_SourceType Int = context.NPF_FRM_SourceType.Where(x => x.Name == Type).FirstOrDefault();
                    if (Int == null)
                    {
                        Int = new NPF_FRM_SourceType()
                        {
                            Name = Type,
                            Description = Type,
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.Now

                        };
                        context.NPF_FRM_SourceType.Add(Int);
                        context.SaveChanges();
                    }
                    List<NPF_FRM_Source> lvl = context.NPF_FRM_Source.Where(x => x.SourceTypeId == Int.Id).ToList();
                    var slvl = InputDetailVall.Where(x => x.Name == Type).FirstOrDefault();
                    if (slvl != null)
                    {
                        foreach (var vs in slvl.lst)
                        {
                            if (!lvl.Any(x => x.Name == vs))
                            {
                                // context.FSM_SER_InputType.Update(Int);
                                NPF_FRM_Source lv = new NPF_FRM_Source()
                                {
                                    Name = vs,
                                    Description = vs,
                                    SourceTypeId = Int.Id,
                                    CreatedBy = createdBy,
                                    CreatedDate = DateTime.Now
                                };
                                context.NPF_FRM_Source.Add(lv);
                                context.SaveChanges();
                            }
                        }
                    }


                }


            }
            catch (Exception e)
            {

            }

        }
        public static async void SeedParametersList(UserManager<ApplicationUser> userManager)
        {
            try
            {
                NPFContext context = new NPFContext();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@muzztech.mu").Select(x => x.Id).FirstOrDefault());

                string Param = "";
                NPF_FRM_ParametersList PL = new NPF_FRM_ParametersList();

                //Table LookUpValue//
                long SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_SYS_LookUpValue").FirstOrDefault().Id;
                //Column
                Param = "LookUpTypeId";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }
                //Column
                Param = "DeleteStatus";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//"Name",
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }

                //Table NAA
                SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_DIR_NAA").FirstOrDefault().Id;
                //Column
                Param = "DeleteStatus";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "FirstName+' '+a.LastName",//Param,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }
                //Column
                Param = "Details";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Details",//Param,
                        SourceId = SourceId,
                        IsComplex = true,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }

                //Table Resource Type
                SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_SYS_ResourceType").FirstOrDefault().Id;
                //Column
                Param = "DeleteStatus";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }

                //Table Station
                SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_SYS_Station").FirstOrDefault().Id;
                //Column
                Param = "DeleteStatus";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }


                //Table Station
                SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_SYS_Resource").FirstOrDefault().Id;
                //Column
                Param = "DeleteStatus";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        IsComplex = true,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }

                //Table Station
                SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_SYS_Resource").FirstOrDefault().Id;
                //Column
                Param = "ResourceTypeId";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        IsComplex = true,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }


                //Table NPF_SYS_User
                SourceId = context.NPF_FRM_Source.Where(x => x.Name == "NPF_SYS_User").FirstOrDefault().Id;
                //Column
                Param = "DeleteStatus";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now,
                        IsComplex = true

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }
                //Column
                Param = "Details";
                PL = context.NPF_FRM_ParametersList.Where(x => x.Name == Param && x.SourceId == SourceId).FirstOrDefault();
                if (PL == null)
                {
                    PL = new NPF_FRM_ParametersList()
                    {
                        Name = Param,
                        Description = "Name",//Param,
                        SourceId = SourceId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now,
                        IsComplex = true

                    };
                    context.NPF_FRM_ParametersList.Add(PL);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }

        }

        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedUpdateUsers(userManager);
            SeedModules(userManager);
            SeedAcessRights(roleManager);
            SeedAcessRightsN(roleManager);
            SeedLookUpType(userManager);
            SeedGlobalParam(userManager);
            SeedTableCodeConfigurations(userManager);
            SeedUOM(userManager);
            SeedResourceType(userManager);
            SeedGroupMatrix(userManager);
            SeedEmailConfig(userManager);
            SeedInputType(userManager);
            SeedSource(userManager);
            SeedParametersList(userManager);
            SeedActivityEventType(userManager);
            SeedStationType(userManager);
            SeedIncidentType(userManager);
          //  SeedAssetMakeModel(userManager);
        }


    }

}

