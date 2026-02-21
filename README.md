
# YoutubeScheduler Windows Desktop App (.Net core 8)

<img width="1197" height="661" alt="image" src="https://github.com/user-attachments/assets/9bd16dae-d41e-4f69-b27a-74cd2981ccac" />

Fitur utama:
- Membuat jadwal streaming otomatis untuk rentang tanggal yang dipilih.
  - Misa Minggu: setiap Minggu jam 10:00
  - Misa Sabtu (Nuansa Karismatik / Kategorial): setiap Sabtu jam 17:00
- Membuat playlist (YouTube) dan live broadcast + live stream secara otomatis.
- Mengubah visibility broadcast (public / private) untuk broadcast yang sudah ada di rentang tanggal.
- Mengambil informasi broadcast dari YouTube dan memperbarui data di grid.
- Mengekspor jadwal Sabtu ke file Excel (EPPlus).

Perilaku penting dari kode (`Form1.cs`):
- OAuth: memakai file `client_secret.json` yang harus tersedia di folder aplikasi (kode membuka nama file ini secara eksplisit).
- Playlist dibuat dengan privacy `public` (lihat `CreateYoutubePlaylistAsync`).
- Broadcast (live) dibuat dengan privacy `private` secara default (lihat `CreateLiveBroadcastAsync`).
- Chat dan beberapa fitur dimatikan/diatur sesuai checkbox pada UI (`checkBoxDisableChat`, `checkBoxDisableMonetize`).
- Judul jadwal dibentuk menggunakan template di metode `AddWeeklySchedulesWithCustomTitles()` (ada placeholder waktu/emoji yang bisa disesuaikan).

Cara pakai singkat:
1. Atur `dateTimePickerStart` dan `dateTimePickerEnd` pada UI.
2. Klik `Tambah Jadwal` untuk menghasilkan daftar jadwal ke grid.
3. Klik `Buat di YouTube` untuk membuat playlist, broadcast, dan stream di YouTube (akan memicu OAuth).
4. Gunakan `Set Public Visibility` / `Set Hidden Visibility` untuk mengubah visibility broadcast pada rentang tanggal.
5. `Get Broadcast Info` mengambil status, broadcastId, streamId untuk rentang tanggal.
6. `Generate Excel` mengekspor jadwal Sabtu ke file `.xlsx`.

Konfigurasi OAuth (ringkas):
1. Buat project di Google Cloud Console.
2. Aktifkan YouTube Data API v3.
3. Buat OAuth Client ID tipe "Desktop app" dan download JSON.
4. Simpan file JSON sebagai `client_secret.json` di folder aplikasi.
5. Tambahkan email Anda sebagai test user pada OAuth consent screen jika project masih dalam mode testing.

Catatan penting dan troubleshooting:
- Pastikan nama file credential adalah `client_secret.json` (kode membuka file ini). Jika README lama menyebut `client_secret_puhsarang.json`, ubah menjadi `client_secret.json` atau ubah kode untuk memakai nama lain.
- Jika muncul peringatan "App isn't verified" saat OAuth, pilih Advanced -> Go to ... (unsafe) untuk melanjutkan saat pengujian.
- Jika mendapat error 403, periksa kembali bahwa email Anda terdaftar sebagai test user dan API sudah diaktifkan. Hapus folder token di `%APPDATA%\\Google.Apis.Auth` jika perlu dan ulangi otorisasi.
- Jika ada error terkait "resolution" saat binding stream, coba gunakan resolusi lebih rendah (kode mencoba 1080p lalu fallback ke 720p).
- Excel export memakai EPPlus; lisensi diatur ke `LicenseContext.NonCommercial`.

Struktur file utama:
- `Form1.cs` - logika utama (YouTube API, pembuatan jadwal, export Excel)
- `Form1.Designer.cs` - definisi UI
- `Form1.resx` - resource

Lisensi dan dependensi:
- Proyek menggunakan Google.Apis.YouTube.v3 dan EPPlus.
- Target framework: .NET 8.
