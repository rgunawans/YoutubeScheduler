using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace YoutubeScheduler
{
    public partial class Form1 : Form
    {
        private List<YoutubeSchedule> schedules = new List<YoutubeSchedule>();
        private BindingSource bindingSource = new BindingSource();
        private Dictionary<string, string> createdPlaylists = new Dictionary<string, string>(); // PlaylistName -> PlaylistId

        public Form1()
        {
            InitializeComponent();
            bindingSource.DataSource = schedules;
            dataGridViewSchedules.DataSource = bindingSource;

            // Tambahkan kolom VisibilityStatus jika belum ada
            if (!dataGridViewSchedules.Columns.Contains("VisibilityStatus"))
            {
                var col = new DataGridViewTextBoxColumn();
                col.DataPropertyName = "VisibilityStatus";
                col.Name = "VisibilityStatus";
                col.HeaderText = "Visibility";
                dataGridViewSchedules.Columns.Add(col);
            }

            // Set default date range
            dateTimePickerStart.Value = DateTime.Today.AddDays(1);
            dateTimePickerEnd.Value = DateTime.Today.AddMonths(1);
            dateTimePickerStart.MinDate = DateTime.Today.AddDays(1);
            dateTimePickerEnd.MinDate = DateTime.Today.AddDays(1);
        }

        private void btnTambahJadwal_Click(object sender, EventArgs e)
        {
            // Validasi range tanggal
            if (dateTimePickerStart.Value > dateTimePickerEnd.Value)
            {
                MessageBox.Show("Tanggal Mulai tidak boleh lebih besar dari Tanggal Akhir!",
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return;
            }

            AddWeeklySchedulesWithCustomTitles();
        }

        private string GetIndonesianMonth(int month)
        {
            return month switch
            {
                1 => "Januari",
                2 => "Februari",
                3 => "Maret",
                4 => "April",
                5 => "Mei",
                6 => "Juni",
                7 => "Juli",
                8 => "Agustus",
                9 => "September",
                10 => "Oktober",
                11 => "November",
                12 => "Desember",
                _ => throw new ArgumentOutOfRangeException(nameof(month))
            };
        }

        private void AddWeeklySchedulesWithCustomTitles()
        {
            schedules.Clear(); // Clear existing schedules
            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    string title = $"Misa Minggu  | ?? Minggu, {date.Day} {GetIndonesianMonth(date.Month)} {date.Year} | ?? 10.00";
                    string playlist = $"Misa Streaming {GetIndonesianMonth(date.Month)} {date.Year}";
                    var schedule = new YoutubeSchedule
                    {
                        Title = title,
                        StartTime = date.Date.AddHours(10), // 10.00
                        Description = title,
                        Playlist = playlist
                    };
                    schedules.Add(schedule);
                }
                else if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    string title = $"Misa Nuansa Karismatik bersama  | ?? Sabtu, {date.Day} {GetIndonesianMonth(date.Month)} {date.Year} | ??17.00";
                    string playlist = $"Misa Kategorial {GetIndonesianMonth(date.Month)} {date.Year}";
                    var schedule = new YoutubeSchedule
                    {
                        Title = title,
                        StartTime = date.Date.AddHours(17), // 17.00
                        Description = title,
                        Playlist = playlist
                    };
                    schedules.Add(schedule);
                }
            }
            bindingSource.ResetBindings(false);
            MessageBox.Show($"Berhasil membuat {schedules.Count} jadwal streaming untuk periode:\n" +
                           $"{startDate:d MMMM yyyy} - {endDate:d MMMM yyyy}");
        }

        private async Task<YouTubeService> GetYouTubeServiceAsync()
        {
            try
            {
                using var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read);
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] {
                        YouTubeService.Scope.YoutubeUpload,
                        YouTubeService.Scope.Youtube
                    },
                    "user",
                    CancellationToken.None
                );

                return new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "YoutubeScheduler"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saat autentikasi YouTube: {ex.Message}\n\n" +
                               "Pastikan:\n" +
                               "1. File client_secret.json ada dan valid\n" +
                               "2. Email Anda ditambahkan sebagai test user di Google Cloud Console\n" +
                               "3. YouTube Data API v3 sudah diaktifkan");
                throw;
            }
        }

        private async Task<string> CreateYoutubePlaylistAsync(YouTubeService youtubeService, string playlistTitle, string description)
        {
            // Check if playlist already created
            if (createdPlaylists.ContainsKey(playlistTitle))
            {
                return createdPlaylists[playlistTitle];
            }

            var newPlaylist = new Playlist
            {
                Snippet = new PlaylistSnippet
                {
                    Title = playlistTitle,
                    Description = description
                },
                Status = new PlaylistStatus
                {
                    PrivacyStatus = "public"
                }
            };

            var request = youtubeService.Playlists.Insert(newPlaylist, "snippet,status");
            var response = await request.ExecuteAsync();
            
            // Cache the created playlist
            createdPlaylists[playlistTitle] = response.Id;
            return response.Id;
        }

        private async Task<string> CreateLiveBroadcastAsync(YouTubeService youtubeService, string title, string description, DateTime scheduledStartTime)
        {
            var broadcast = new LiveBroadcast
            {
                Snippet = new LiveBroadcastSnippet
                {
                    Title = title,
                    Description = description,
                    ScheduledStartTimeDateTimeOffset = new DateTimeOffset(scheduledStartTime),
                    ScheduledEndTimeDateTimeOffset = new DateTimeOffset(scheduledStartTime.AddHours(2)) // Default 2 hours duration
                },
                Status = new LiveBroadcastStatus
                {
                    PrivacyStatus = "private", // Changed to private for hidden visibility initially
                    SelfDeclaredMadeForKids = checkBoxDisableMonetize.Checked // Set ini saat pembuatan
                },
                ContentDetails = new LiveBroadcastContentDetails
                {
                    EnableDvr = true,
                    EnableContentEncryption = false,
                    EnableEmbed = true,
                    RecordFromStart = true,
                    StartWithSlate = false,
                    EnableAutoStart = true,  // Enable auto start
                    EnableAutoStop = true,   // Enable auto stop
                    EnableClosedCaptions = false, // Disable closed captions
                    MonitorStream = new MonitorStreamInfo
                    {
                        EnableMonitorStream = !checkBoxDisableChat.Checked,
                        BroadcastStreamDelayMs = 0
                    }
                }
            };

            var request = youtubeService.LiveBroadcasts.Insert(broadcast, "snippet,status,contentDetails");
            var response = await request.ExecuteAsync();

            // Tidak perlu update MadeForKids lagi karena sudah diatur saat pembuatan
            return response.Id; // Broadcast ID
        }

        private async Task<string> CreateLiveStreamAsync(YouTubeService youtubeService, string title)
        {
            try
            {
                // Try with 1080p first
                var stream = new LiveStream
                {
                    Snippet = new LiveStreamSnippet
                    {
                        Title = title
                    },
                    Cdn = new CdnSettings
                    {
                        Format = "1080p",
                        IngestionType = "rtmp",
                        Resolution = "1080p",
                        FrameRate = "60fps"
                    },
                    ContentDetails = new LiveStreamContentDetails
                    {
                        IsReusable = false
                    }
                };

                var request = youtubeService.LiveStreams.Insert(stream, "snippet,cdn,contentDetails");
                var response = await request.ExecuteAsync();
                return response.Id;
            }
            catch (Exception)
            {
                // Fallback to 720p if 1080p fails
                var stream = new LiveStream
                {
                    Snippet = new LiveStreamSnippet
                    {
                        Title = title + " (720p)"
                    },
                    Cdn = new CdnSettings
                    {
                        Format = "720p",
                        IngestionType = "rtmp",
                        Resolution = "720p",
                        FrameRate = "60fps"
                    },
                    ContentDetails = new LiveStreamContentDetails
                    {
                        IsReusable = false
                    }
                };

                var request = youtubeService.LiveStreams.Insert(stream, "snippet,cdn,contentDetails");
                var response = await request.ExecuteAsync();
                return response.Id;
            }
        }

        private async Task<bool> ValidateStreamAsync(YouTubeService youtubeService, string streamId)
        {
            try
            {
                var request = youtubeService.LiveStreams.List("snippet,cdn,status");
                request.Id = streamId;
                var response = await request.ExecuteAsync();
                
                if (response.Items.Count > 0)
                {
                    var stream = response.Items[0];
                    return stream.Status.StreamStatus == "ready";
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task BindBroadcastToStreamAsync(YouTubeService youtubeService, string broadcastId, string streamId)
        {
            try
            {
                // Wait a bit to ensure stream is ready
                await Task.Delay(1000);

                var bindRequest = youtubeService.LiveBroadcasts.Bind(broadcastId, "id,contentDetails");
                bindRequest.StreamId = streamId;
                await bindRequest.ExecuteAsync();
            }
            catch (Exception ex)
            {
                // If binding fails, try with different approach
                if (ex.Message.Contains("resolution") || ex.Message.Contains("format"))
                {
                    throw new Exception($"Stream resolution/format error: {ex.Message}. Try using a different resolution or check your stream settings.");
                }
                throw;
            }
        }

        private async Task AddVideoToPlaylistAsync(YouTubeService youtubeService, string playlistId, string videoId)
        {
            var playlistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playlistId,
                    ResourceId = new ResourceId
                    {
                        Kind = "youtube#video",
                        VideoId = videoId
                    }
                }
            };

            var request = youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
            await request.ExecuteAsync();
        }

        private async void btnCreateYoutube_Click(object sender, EventArgs e)
        {
            if (schedules.Count ==0)
            {
                MessageBox.Show("Tidak ada jadwal yang dibuat. Silakan klik 'Tambah Jadwal' terlebih dahulu.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "PERHATIAN: Aplikasi ini memerlukan akses ke YouTube API.\n\n" +
                "Jika muncul peringatan 'App isn't verified', pilih:\n" +
                "1. 'Advanced' atau 'Show Advanced'\n" +
                "2. 'Go to YoutubeScheduler (unsafe)'\n" +
                "3. Lanjutkan dengan 'Allow'\n\n" +
                "Pastikan email Anda sudah ditambahkan sebagai test user di Google Cloud Console.\n\n" +
                "Lanjutkan?",
                "Konfirmasi YouTube API Access",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                btnCreateYoutube.Enabled = false;
                btnCreateYoutube.Text = "Membuat...";

                var youtubeService = await GetYouTubeServiceAsync();
                createdPlaylists.Clear();
                int successCount =0;
                int totalSchedules = schedules.Count;
                var errorList = new List<string>();

                for (int i =0; i < schedules.Count; i++)
                {
                    var schedule = schedules[i];
                    try
                    {
                        string playlistId = await CreateYoutubePlaylistAsync(youtubeService, schedule.Playlist, schedule.Playlist);
                        string broadcastId = await CreateLiveBroadcastAsync(youtubeService, schedule.Title, schedule.Description, schedule.StartTime);
                        string streamId = await CreateLiveStreamAsync(youtubeService, schedule.Title);
                        bool streamReady = await ValidateStreamAsync(youtubeService, streamId);
                        if (!streamReady)
                        {
                            await Task.Delay(2000);
                        }
                        await BindBroadcastToStreamAsync(youtubeService, broadcastId, streamId);
                        schedule.BroadcastId = broadcastId;
                        schedule.StreamId = streamId;
                        schedule.PlaylistId = playlistId;
                        schedule.VisibilityStatus = "private"; // Set hidden by default
                        await AddVideoToPlaylistAsync(youtubeService, playlistId, broadcastId);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = ex.Message;
                        if (ex.Message.Contains("resolution"))
                        {
                            errorMsg = "Error: Stream resolution tidak didukung. Coba gunakan resolusi yang lebih rendah.";
                        }
                        else if (ex.Message.Contains("quota"))
                        {
                            errorMsg = "Error: Quota YouTube API sudah habis. Coba lagi besok.";
                        }
                        errorList.Add($"Error creating schedule '{schedule.Title}': {errorMsg}");
                        ShowErrorList(errorList); // Update errorTextBox in realtime
                    }
                    // Update DataGridView after each schedule processed
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => bindingSource.ResetBindings(false)));
                    }
                    else
                    {
                        bindingSource.ResetBindings(false);
                    }
                }

                // Show summary in errorTextBox only
                var summaryMsg = $"Selesai! Berhasil membuat {successCount} dari {totalSchedules} jadwal streaming.\n" +
                                 $"Total playlist yang dibuat: {createdPlaylists.Count}\n" +
                                 $"Playlist yang dibuat:\n{string.Join("\n", createdPlaylists.Keys)}";
                if (errorList.Count >0)
                {
                    summaryMsg += "\n\nAda error pada beberapa jadwal. Detail error dapat dilihat di bawah.";
                }
                errorList.Insert(0, summaryMsg);
                ShowErrorList(errorList);

                // Only show simple success/failure popup
                if (successCount == totalSchedules && errorList.Count ==1)
                {
                    MessageBox.Show("Berhasil dibuat.");
                }
                else
                {
                    MessageBox.Show("Gagal membuat semua jadwal.");
                }
            }
            catch (Exception ex)
            {
                string errorDetails = ex.Message;
                if (ex.Message.Contains("resolution"))
                {
                    errorDetails += "\n\nSolusi:\n" +
                                   "1. Pastikan channel YouTube mendukung live streaming\n" +
                                   "2. Verifikasi nomor telepon di YouTube\n" +
                                   "3. Channel harus tidak memiliki batasan live streaming dalam90 hari terakhir";
                }
                ShowErrorList(new List<string> { $"Error: {errorDetails}" });
                MessageBox.Show("Gagal membuat semua jadwal.");
            }
            finally
            {
                btnCreateYoutube.Enabled = true;
                btnCreateYoutube.Text = "Buat di YouTube";
            }
        }

        

      

        private async Task UpdateBroadcastVisibilityAsync(YouTubeService youtubeService, string broadcastId, string privacyStatus)
        {
            var broadcast = new LiveBroadcast
            {
                Id = broadcastId,
                Status = new LiveBroadcastStatus
                {
                    PrivacyStatus = privacyStatus
                }
            };

            var request = youtubeService.LiveBroadcasts.Update(broadcast, "status");
            await request.ExecuteAsync();
        }

        private async void btnSetPublicVisibility_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "PERHATIAN: Aplikasi ini akan mengubah visibility broadcast menjadi public untuk semua jadwal yang sudah ada di YouTube dalam range tanggal yang dipilih.\n\n" +
                "Pastikan broadcast sudah dibuat dan Anda memiliki akses.\n\n" +
                "Lanjutkan?",
                "Konfirmasi Set Public Visibility",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                btnSetPublicVisibility.Enabled = false;
                btnSetPublicVisibility.Text = "Mengubah...";

                var youtubeService = await GetYouTubeServiceAsync();
                DateTime startDate = dateTimePickerStart.Value.Date;
                DateTime endDate = dateTimePickerEnd.Value.Date;

                var broadcastIds = await GetBroadcastIdsFromYoutubeAsync(youtubeService, startDate, endDate);
                int successCount = 0;
                int totalSchedules = broadcastIds.Count;
                var errorList = new List<string>();

                foreach (var broadcastId in broadcastIds)
                {
                    try
                    {
                        await UpdateBroadcastVisibilityAsync(youtubeService, broadcastId, "public");
                        successCount++;
                        // Update status di schedules jika ada
                        var schedule = schedules.FirstOrDefault(s => s.BroadcastId == broadcastId);
                        if (schedule != null)
                        {
                            schedule.VisibilityStatus = "public";
                        }
                    }
                    catch (Exception ex)
                    {
                        errorList.Add($"Error updating broadcast '{broadcastId}': {ex.Message}");
                        ShowErrorList(errorList);
                    }
                }
                bindingSource.ResetBindings(false);

                // Show summary in errorTextBox only
                var summaryMsg = $"Selesai! Berhasil mengubah visibility {successCount} dari {totalSchedules} broadcast ke public.\n" +
                                 $"Range tanggal: {startDate:d MMMM yyyy} - {endDate:d MMMM yyyy}";
                if (errorList.Count >0)
                {
                    summaryMsg += "\n\nAda error pada beberapa broadcast. Detail error dapat dilihat di bawah.";
                }
                errorList.Insert(0, summaryMsg);
                ShowErrorList(errorList);

                // Only show simple success/failure popup
                if (successCount == totalSchedules && errorList.Count ==1)
                {
                    MessageBox.Show("Berhasil dibuat.");
                }
                else
                {
                    MessageBox.Show("Gagal mengubah visibility.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorList(new List<string> { $"Error: {ex.Message}" });
                MessageBox.Show("Gagal mengubah visibility.");
            }
            finally
            {
                btnSetPublicVisibility.Enabled = true;
                btnSetPublicVisibility.Text = "Set Public Visibility";
            }
        }

        private async void btnSetHiddenVisibility_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "PERHATIAN: Aplikasi ini akan mengubah visibility broadcast menjadi hidden (private) untuk semua jadwal yang sudah ada di YouTube dalam range tanggal yang dipilih.\n\n" +
                "Pastikan broadcast sudah dibuat dan Anda memiliki akses.\n\n" +
                "Lanjutkan?",
                "Konfirmasi Set Hidden Visibility",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                btnSetHiddenVisibility.Enabled = false;
                btnSetHiddenVisibility.Text = "Mengubah...";

                var youtubeService = await GetYouTubeServiceAsync();
                DateTime startDate = dateTimePickerStart.Value.Date;
                DateTime endDate = dateTimePickerEnd.Value.Date;

                var broadcastIds = await GetBroadcastIdsFromYoutubeAsync(youtubeService, startDate, endDate);
                int successCount = 0;
                int totalSchedules = broadcastIds.Count;
                var errorList = new List<string>();

                foreach (var broadcastId in broadcastIds)
                {
                    try
                    {
                        await UpdateBroadcastVisibilityAsync(youtubeService, broadcastId, "private");
                        successCount++;
                        // Update status di schedules jika ada
                        var schedule = schedules.FirstOrDefault(s => s.BroadcastId == broadcastId);
                        if (schedule != null)
                        {
                            schedule.VisibilityStatus = "private";
                        }
                    }
                    catch (Exception ex)
                    {
                        errorList.Add($"Error updating broadcast '{broadcastId}': {ex.Message}");
                        ShowErrorList(errorList); // Update errorTextBox in realtime
                    }
                }

                // Show summary in errorTextBox only
                var summaryMsg = $"Selesai! Berhasil mengubah visibility {successCount} dari {totalSchedules} broadcast ke hidden (private).\n" +
                                 $"Range tanggal: {startDate:d MMMM yyyy} - {endDate:d MMMM yyyy}";
                if (errorList.Count >0)
                {
                    summaryMsg += "\n\nAda error pada beberapa broadcast. Detail error dapat dilihat di bawah.";
                }
                errorList.Insert(0, summaryMsg);
                ShowErrorList(errorList);

                // Only show simple success/failure popup
                if (successCount == totalSchedules && errorList.Count ==1)
                {
                    MessageBox.Show("Berhasil dibuat.");
                }
                else
                {
                    MessageBox.Show("Gagal mengubah visibility.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorList(new List<string> { $"Error: {ex.Message}" });
                MessageBox.Show("Gagal mengubah visibility.");
            }
            finally
            {
                btnSetHiddenVisibility.Enabled = true;
                btnSetHiddenVisibility.Text = "Set Hidden Visibility";
            }
        }

        private void ShowErrorList(List<string> errorList)
        {
            if (errorTextBox.InvokeRequired)
            {
                errorTextBox.Invoke(new Action(() => errorTextBox.Text = string.Join("\r\n", errorList)));
            }
            else
            {
                errorTextBox.Text = string.Join("\r\n", errorList);
            }
        }

        private async Task<List<string>> GetBroadcastIdsFromYoutubeAsync(YouTubeService youtubeService, DateTime startDate, DateTime endDate)
        {
            var broadcastIds = new List<string>();
            var request = youtubeService.LiveBroadcasts.List("id,snippet,status");
            request.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
            request.Mine = true;
            request.MaxResults =50;

            var response = await request.ExecuteAsync();
            foreach (var item in response.Items)
            {
                if (item.Snippet.ScheduledStartTime.HasValue)
                {
                    var scheduled = item.Snippet.ScheduledStartTime.Value.Date;
                    if (scheduled >= startDate.Date && scheduled <= endDate.Date)
                    {
                        broadcastIds.Add(item.Id);
                    }
                }
            }
            return broadcastIds;
        }

        private async void btnGetBroadcastInfo_Click(object sender, EventArgs e)
        {
            btnGetBroadcastInfo.Enabled = false;
            btnGetBroadcastInfo.Text = "Mengambil...";
            try
            {
                var youtubeService = await GetYouTubeServiceAsync();
                DateTime startDate = dateTimePickerStart.Value.Date;
                DateTime endDate = dateTimePickerEnd.Value.Date;
                var request = youtubeService.LiveBroadcasts.List("id,snippet,contentDetails,status");
                request.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
                request.Mine = true;
                request.MaxResults =50;
                var response = await request.ExecuteAsync();
                var infoList = new List<string>();
                foreach (var item in response.Items)
                {
                    if (item.Snippet.ScheduledStartTime.HasValue)
                    {
                        var scheduled = item.Snippet.ScheduledStartTime.Value.Date;
                        if (scheduled >= startDate && scheduled <= endDate)
                        {
                            string broadcastId = item.Id;
                            string visibility = item.Status?.PrivacyStatus ?? "";
                            string streamId = item.ContentDetails?.BoundStreamId ?? "";
                            // Cari jadwal yang cocok berdasarkan tanggal dan update datanya
                            var schedule = schedules.FirstOrDefault(s => s.StartTime.Date == scheduled);
                            if (schedule != null)
                            {
                                schedule.BroadcastId = broadcastId;
                                schedule.StreamId = streamId;
                                schedule.VisibilityStatus = visibility;
                                // PlaylistId tetap dari jadwal, jika ada
                            }
                            string playlistId = schedule?.PlaylistId ?? "";
                            infoList.Add($"BroadcastId: {broadcastId}\nStreamId: {streamId}\nPlaylistId: {playlistId}\nVisibility: {visibility}\n---");
                        }
                    }
                }
                bindingSource.ResetBindings(false); // Update grid
                if (infoList.Count ==0)
                {
                    MessageBox.Show("Berhasil dibuat.");
                }
                else
                {
                    ShowErrorList(infoList);
                    MessageBox.Show("Berhasil dibuat.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorList(new List<string> { $"Error: {ex.Message}" });
                MessageBox.Show("Gagal mengambil info broadcast.");
            }
            finally
            {
                btnGetBroadcastInfo.Enabled = true;
                btnGetBroadcastInfo.Text = "Get Broadcast Info";
            }
        }

        private async void btnGenerateExcel_Click(object sender, EventArgs e)
        {
            try
      {
           // Set EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

  DateTime startDate = dateTimePickerStart.Value.Date;
      DateTime endDate = dateTimePickerEnd.Value.Date;

                // Filter jadwal Sabtu saja
   var saturdaySchedules = schedules
     .Where(s => s.StartTime.DayOfWeek == DayOfWeek.Saturday && 
  s.StartTime.Date >= startDate && 
     s.StartTime.Date <= endDate)
        .OrderBy(s => s.StartTime)
         .ToList();

          if (saturdaySchedules.Count == 0)
            {
     MessageBox.Show("Tidak ada jadwal Sabtu dalam range tanggal yang dipilih.", 
     "Info", 
       MessageBoxButtons.OK, 
    MessageBoxIcon.Information);
  return;
    }

                // Ambil Playlist IDs dari YouTube
        var youtubeService = await GetYouTubeServiceAsync();
 var playlistMapping = await GetPlaylistIdsFromYouTubeAsync(youtubeService);

     // Buat SaveFileDialog
          using var saveFileDialog = new SaveFileDialog();
           saveFileDialog.Filter = "Excel Files|*.xlsx";
     saveFileDialog.Title = "Simpan Jadwal Sabtu";
    saveFileDialog.FileName = $"Jadwal_Sabtu_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";

          if (saveFileDialog.ShowDialog() == DialogResult.OK)
      {
         using var package = new ExcelPackage();
           var worksheet = package.Workbook.Worksheets.Add("Jadwal Sabtu");

          int currentRow = 1;

 // Group by month and year
     var groupedByMonth = saturdaySchedules
         .GroupBy(s => new { s.StartTime.Year, s.StartTime.Month })
           .OrderBy(g => g.Key.Year)
  .ThenBy(g => g.Key.Month);

        foreach (var monthGroup in groupedByMonth)
          {
             // Header bulan
  worksheet.Cells[currentRow, 1].Value = GetIndonesianMonth(monthGroup.Key.Month);
   worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                 worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
         worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
   worksheet.Cells[currentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                 // Playlist ID dan URL
  var firstScheduleInMonth = monthGroup.First();
    string playlistId = string.Empty;
              
        // Cek dari schedules dulu (jika sudah dibuat via app)
         if (!string.IsNullOrEmpty(firstScheduleInMonth.PlaylistId))
     {
             playlistId = firstScheduleInMonth.PlaylistId;
 }
        // Jika tidak ada, cari dari YouTube berdasarkan nama playlist
    else if (playlistMapping.ContainsKey(firstScheduleInMonth.Playlist))
      {
                playlistId = playlistMapping[firstScheduleInMonth.Playlist];
                  }

         // Kolom 2: URL Playlist langsung sebagai hyperlink
       if (!string.IsNullOrEmpty(playlistId))
        {
 string playlistUrl = $"https://www.youtube.com/playlist?list={playlistId}";
     worksheet.Cells[currentRow, 2].Formula = $"=HYPERLINK(\"{playlistUrl}\", \"{playlistUrl}\")";
       worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
      worksheet.Cells[currentRow, 2].Style.Font.Color.SetColor(Color.Blue);
          worksheet.Cells[currentRow, 2].Style.Font.UnderLine = true;
        worksheet.Cells[currentRow, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
   worksheet.Cells[currentRow, 2].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
  worksheet.Cells[currentRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
      }

       currentRow++;

            // Tanggal-tanggal
foreach (var schedule in monthGroup.OrderBy(s => s.StartTime))
                  {
        worksheet.Cells[currentRow, 1].Value = schedule.StartTime.Day;
                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
  worksheet.Cells[currentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
      currentRow++;
      }

    // Baris kosong antara bulan
       currentRow++;
}

            // Set column width
      worksheet.Column(1).Width = 20;
           worksheet.Column(2).Width = 60;

    // Simpan file
          await package.SaveAsAsync(new FileInfo(saveFileDialog.FileName));

        MessageBox.Show($"File Excel berhasil dibuat!\n{saveFileDialog.FileName}", 
   "Sukses", 
     MessageBoxButtons.OK, 
   MessageBoxIcon.Information);

  // Buka file Excel
         System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = saveFileDialog.FileName,
      UseShellExecute = true
     });
             }
    }
   catch (Exception ex)
            {
 MessageBox.Show($"Error saat membuat Excel: {ex.Message}", 
     "Error", 
     MessageBoxButtons.OK, 
           MessageBoxIcon.Error);
     }
   }

 private async Task<Dictionary<string, string>> GetPlaylistIdsFromYouTubeAsync(YouTubeService youtubeService)
        {
          var playlistMapping = new Dictionary<string, string>();
    
       try
{
      var request = youtubeService.Playlists.List("snippet");
        request.Mine = true;
      request.MaxResults = 50;

   var response = await request.ExecuteAsync();
           
           foreach (var playlist in response.Items)
      {
             playlistMapping[playlist.Snippet.Title] = playlist.Id;
        }
            }
            catch (Exception ex)
   {
     // Jika gagal, tidak masalah, playlist ID akan kosong
      System.Diagnostics.Debug.WriteLine($"Warning: Could not fetch playlists from YouTube: {ex.Message}");
            }

 return playlistMapping;
        }
    }

    public class YoutubeSchedule
    {
public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
 public string Description { get; set; } = string.Empty;
    public string Playlist { get; set; } = string.Empty;
        public string? BroadcastId { get; set; }
        public string? StreamId { get; set; }
      public string? PlaylistId { get; set; }
        public string VisibilityStatus { get; set; } = "private"; // default hidden
    }
}

