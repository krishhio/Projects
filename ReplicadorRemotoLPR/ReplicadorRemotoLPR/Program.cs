using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Net;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ReplicadorRemotoLPR
{
    class Program
    {
        ISS.Net.IidkManager iidkManager = new ISS.Net.IidkManager();
        static void Main(string[] args)
        {
            new Program();
        }

        Program()
        {
            iidkManager.OnConnectionStateChanged += new ConnectionStateListener(ConnectionStateChanged);
            iidkManager.OnSecurOSMessage += new MessagesListener(SecurOSMessage);
            iidkManager.Connect("127.0.0.1", "1030", "1");
            //MainAsyncSelect();
            string Recibido;
            while (true)
            {
                Recibido = Console.ReadLine();

                if (Recibido == "salir")
                {
                    iidkManager.Disconnect();
                    break;
                }
            }
        }
        async Task<string> MainAsyncSelect()
        {
            var cliente = new MongoClient();
            var db = cliente.GetDatabase("Auto");
            var coleccion = db.GetCollection<BsonDocument>("t_Log");
            var lista = await coleccion.Find(new BsonDocument()).ToListAsync();
            List<Auto> autos = new List<Auto>();
            foreach (var doc in lista)
            {
                Console.WriteLine(doc);
            }
            return autos.ToString();
        }

        async Task MainAsyncInsertOne(Auto car)
        {
            var cliente = new MongoClient();
            var db = cliente.GetDatabase("Auto");
            var coleccion = db.GetCollection<BsonDocument>("t_Log");
            var auto = new BsonDocument
            {
                {"best_view_date_time", car.best_view_date_time},
                { "velocity", car.velocity},
                { "direction_id", car.direction_id},
                { "best_view_mask_id", car.best_view_mask_id},
                { "number_utf8", car.number_utf8},
                { "camera_id", car.camera_id},
                { "direction_name", car.direction_name},
                { "number", car.number},
                { "plate_bottom_i", car.plate_bottom_i},
                { "plate_left_i", car.plate_left_i},
                { "plate_right_i", car.plate_right_i},
                { "plate_top_i", car.plate_top_i},
                { "recognizer_id", car.recognizer_id},
                { "recognizer_name", car.recognizer_name},
                { "time_leave", car.time_leave},
                { "recognizer_type", car.recognizer_type},
                { "speed", car.speed},
                { "template_country_id", car.template_country_id},
                { "time", car.time},
                { "template_country_iso_code", car.template_country_iso_code},
                { "core_global", car.core_global},
                { "template_country_name", car.template_country_name},
                { "template_name", car.template_name},
                { "time_enter", car.time_enter},
                { "units", car.units},
                { "track_id", car.track_id},
                { "tracked_out", car.tracked_out},
                { "weight", car.weight},
                { "_generated_slave_id", car._generated_slave_id},
                { "slave_id", car.slave_id},
                { "owner", car.owner},
                { "date", car.date},
                { "estatus", car.estatus},
                { "imagen", car.imagen},
                { "guid", car.guid},
                { "nImagen", car.nImagen}
            };
            await coleccion.InsertOneAsync(auto);

        }

        async Task MainAsyncUpdate(string idRequest)
        {
            string imagenBase64 = image2Base64(@"C:\placas\" + idRequest+".jpg");
            var cliente = new MongoClient();
            var db = cliente.GetDatabase("Auto");
            var coleccion = db.GetCollection<BsonDocument>("t_Log");
            var filter = Builders<BsonDocument>.Filter.Eq("guid", idRequest);
            var update = Builders<BsonDocument>.Update.Set("imagen", imagenBase64);
            var result = await coleccion.UpdateOneAsync(filter, update);
        }

        private void ConnectionStateChanged(bool connected)
        {

        }

        private void SecurOSMessage(Message msg)
        {
            //Console.WriteLine(msg);
            string urlFileJPG = @"C:\placas\";
            if (msg.GetMessageAction() == "EVENT" && msg.GetParam("objtype") == "LPR_CAM" || msg.GetParam("objtype") == "LPR_CAM_LITE" && msg.GetParam("objaction") == "CAR_LP_RECOGNIZED")
            {
                Auto car = new Auto();
                car.best_view_date_time = msg.GetParam("best_view_date_time");
                car.velocity = msg.GetParam("velocity");
                car.direction_id = msg.GetParam("direction_id");
                car.best_view_mask_id = msg.GetParam("best_view_mask_id");
                car.number_utf8 = msg.GetParam("number_utf8");
                car.camera_id = msg.GetParam("camera_id");
                car.direction_name = msg.GetParam("direction_name");
                car.number = msg.GetParam("number");
                car.plate_bottom_i = msg.GetParam("plate_bottom_i");
                car.plate_left_i = msg.GetParam("plate_left_i");
                car.plate_right_i = msg.GetParam("plate_right_i");
                car.plate_top_i = msg.GetParam("plate_top_i");
                car.recognizer_id = msg.GetParam("recognizer_id");
                car.recognizer_name = msg.GetParam("recognizer_name");
                car.time_leave = msg.GetParam("time_leave");
                car.recognizer_type = msg.GetParam("recognizer_type");
                car.speed = msg.GetParam("speed");
                car.template_country_id = msg.GetParam("template_country_id");
                car.time = msg.GetParam("time");
                car.template_country_iso_code = msg.GetParam("template_country_iso_code");
                car.core_global = msg.GetParam("core_global");
                car.template_country_name = msg.GetParam("template_country_name");
                car.template_name = msg.GetParam("template_name");
                car.time_enter = msg.GetParam("time_enter");
                car.units = msg.GetParam("units");
                car.track_id = msg.GetParam("track_id");
                car.tracked_out = msg.GetParam("tracked_out");
                car.weight = msg.GetParam("weight");
                car._generated_slave_id = msg.GetParam("_generated_slave_id");
                car.slave_id = msg.GetParam("slave_id");
                car.owner = msg.GetParam("owner");
                car.date = msg.GetParam("date");
                car.estatus = "f";
                car.imagen = "";
                car.guid = Guid.NewGuid().ToString();
                car.nImagen = "";
                MainAsyncInsertOne(car).GetAwaiter();
                string texto = "CORE||DO_REACT|source_type<IMAGE_EXPORT>,source_id<1>,action<EXPORT>,params<4>,param0_name<import>,param0_val<cam$" + car.camera_id + ";time$" + car.best_view_date_time + ">,param1_name<export_engine>,param1_val<file>,param2_name<export>,param2_val<filename$" + car.guid +  @";dir$" + urlFileJPG + ">,param3_name<request_id>,param3_val<"+car.guid+">,param4_name<export_image>,param4_val<format$jpg;quality$70>";
                iidkManager.SendMessage(texto);
                Console.WriteLine("Lectura de placa " + msg.GetParam("number"));
            }
            if (msg.GetMessageAction() == "EVENT" && msg.GetParam("objtype") == "IMAGE_EXPORT" && msg.GetParam("action") == "EXPORT_DONE")
            {
                var requestID = msg.GetParam("request_id");
                MainAsyncUpdate(requestID).GetAwaiter();

            }
        }

        private string image2Base64(string path)
        {
            try
            {
                Console.WriteLine("Codigo de Imagen: " + path);
                string test = path;
                byte[] imageArray = System.IO.File.ReadAllBytes(path);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return base64ImageRepresentation;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return "ERROR";
            }
           
        }
    }
}
