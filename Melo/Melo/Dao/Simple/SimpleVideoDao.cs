﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Melo.Service.Interface;
using Melo.ClientEntities;
using Melo.Dao.Interface;
using log4net;

namespace Melo.Dao.Simple
{
    class SimpleVideoDao : IVideoDao
    {
        private IConnectionCreater ConnectionCreater;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SimpleVideoDao(IConnectionCreater connectionCreater)
        {
            this.ConnectionCreater = connectionCreater;
        }

        public void DeleteVideoById(int id)
        {
            try
            {
                using (SqlConnection conn = ConnectionCreater.connect())
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("Delete FROM videos WHERE id = @0", conn);
                    command.Parameters.Add(new SqlParameter("0", id));
                    command.ExecuteNonQuery();
                    log.Info("Video with the id: " + id + " successfully deleted from the database");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                log.Error("Sql exception occured while deleting a video from the database", e);
            }
        }

        public List<Video> GetAll()
        {
            List<Video> videos = new List<Video>();
            try
            {
                using (SqlConnection conn = ConnectionCreater.connect())
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM videos", conn);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int newid = reader.GetInt32(0);
                            String path = reader.GetString(2);
                            String name = reader.GetString(3);
                            Video video = new Video(path,name,newid);
                            videos.Add(video);

                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                log.Error("Sql exception occured while getting all videos from the database", e);
            }
            return videos;
        }

        public Video GetVideoById(int id)
        {
            Video video = null;
            try
            {
                using (SqlConnection conn = ConnectionCreater.connect())
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM videos WHERE id = @0", conn);
                    command.Parameters.Add(new SqlParameter("0", id));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int newid = reader.GetInt32(0);
                            String path = reader.GetString(2);
                            String name = reader.GetString(3);
                            video = new Video(path, name, newid);

                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                log.Error("Sql exception occured while getting a video from the database", e);
            }
            return video;
        }

        public void InsertVideo(Video video, int directoryId)
        {
            try
            {
                using (SqlConnection conn = ConnectionCreater.connect())
                {
                    conn.Open();
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO videos (name, file_path, extension, directory_id) VALUES (@name, @file_path, @extension, @directory_id)", conn);

                    insertCommand.Parameters.Add(new SqlParameter("name", video.Name));
                    insertCommand.Parameters.Add(new SqlParameter("file_path", video.FilePath));
                    insertCommand.Parameters.Add(new SqlParameter("extension", video.Extension));
                    insertCommand.Parameters.Add(new SqlParameter("directory_id", directoryId));
                    insertCommand.ExecuteNonQuery();
                    log.Info("Video with the name: " + video.Name + " successfully added to the database");


                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                log.Error("Sql exception occured while adding a video to the database", e);
            }
        }
    }
}
