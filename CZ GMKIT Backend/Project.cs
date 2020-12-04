using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MRK {
    public class Project {
        public const string PROJECT_NAME = "CZGMProject.cz";
        public const int PROJECT_PATCH_VERSION = 1;

        public string Path { get; private set; }
        public int PatchVersion { get; private set; }
        public string Magic { get; private set; }
        public DateTime Date { get; private set; }
        public List<Account> Accounts { get; private set; }

        public static bool ParseProject(string path, out Project project) {
            using (FileStream stream = new FileStream($@"{path}\{PROJECT_NAME}", FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream)) {
                try {
                    int pver = reader.ReadInt32();
                    string magic = reader.ReadString();
                    DateTime date = new DateTime(reader.ReadInt64());

                    //int accCount = reader.ReadInt32();
                    List<Account> accs = new List<Account>();
                    /*for (int i = 0; i < accCount; i++) {

                    }*/

                    project = new Project {
                        Path = path,
                        PatchVersion = pver,
                        Magic = magic,
                        Date = date,
                        Accounts = accs
                    };

                    reader.Close();
                }
                catch {
                    project = null;
                    return false;
                }

                return true;
            }
        }

        public static Project PatchProject(string path) {
            using (FileStream stream = new FileStream($@"{path}\{PROJECT_NAME}", FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                Project project = new Project {
                    Path = path,
                    PatchVersion = PROJECT_PATCH_VERSION,
                    Magic = CalculateMagic(),
                    Date = DateTime.Now,
                    Accounts = new List<Account>()
                };

                writer.Write(project.PatchVersion);
                writer.Write(project.Magic);
                writer.Write(project.Date.Ticks);

                writer.Close();

                MRKNative.PatchTest(project);

                return project;
            }
        }

        static string CalculateMagic() {
            return "mg01";
        }
    }
}
