using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.ComponentModel;

public class RegistrationForm
{
    [Key]
    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FormId { get; set; }

    [NotMapped]
    [Display(Name = "Số phiếu")]
    public int FormNumber => 20_000_000 + FormId;

    #region Basic Information

    [Display(Name = "Họ và tên")]
    [Required(AllowEmptyStrings = false)]
    public string FullName { get; set; }

    [Required]
    [Display(Name = "Giới tính")]
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    [Display(Name = "CMND")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 10, MinimumLength = 10)]
    public string CitizenId { get; set; }

    [JsonIgnore]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày cấp")]
    public DateTime? ProvideDate { get; set; }

    [Required]
    [NotMapped]
    [JsonPropertyName("ProvideDate")]
    public long? ProvideDateInUnix
    {
        get => DateTimeToUnixTime(ProvideDate);
        set => ProvideDate = UnixTimeToDateTime(value);
    }

    [Display(Name = "Nơi cấp")]
    [Required(AllowEmptyStrings = false)]
    public string ProvideLocation { get; set; }

    [Phone]
    [Display(Name = "Số điện thoại")]
    [DataType(DataType.PhoneNumber)]
    [Required(AllowEmptyStrings = false)]
    public string PhoneNumber { get; set; }

#nullable enable

    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

#nullable disable

    [Required]
    [Display(Name = "Địa chỉ")]
    public virtual HomeAddress Address { get; set; }

    #endregion

    #region Education Information

    [Display(Name = "Năm tốt nghiệp")]
    [Range(minimum: 1970, maximum: 2020)]
    [Required]
    public int GraduationYear { get; set; }

    [Required]
    [Display(Name = "Kết quả lớp 10")]
    public virtual HighSchoolResult GradeTen { get; set; }

    [Required]
    [Display(Name = "Kết quả lớp 11")]
    public virtual HighSchoolResult GradeEleven { get; set; }

    [Required]
    [Display(Name = "Kết quả lớp 12")]
    public virtual HighSchoolResult GradeTwelve { get; set; }

    [Required]
    [MaxLength(4)]
    [MinLength(1)]
    [Display(Name = "Nhóm ngành")]
    public virtual AssignCareer[] Careers { get; set; }

    [DefaultValue(false)]
    [Display(Name = "Đã hoặc đang học tại Hoa Sen")]
    public bool AlreadyAtHoaSenUniversity { get; set; }

#nullable enable

    [Display(Name = "MSSV cũ")]
    [StringLength(maximumLength: 7, MinimumLength = 7)]
    public string? OldStudentId { get; set; }

    [DefaultValue(false)]
    [Display(Name = "Hồ sơ gồm học bạ hoặc sổ liên lạc có điểm.")]
    public bool IncludeSchoolProfile { get; set; }

#nullable disable

    #endregion

    [JsonIgnore]
    public DateTime CreationDate { get; set; }

    [NotMapped]
    public long? CreationDateInUnix => DateTimeToUnixTime(CreationDate);

    [JsonIgnore]
    public DateTime RecentEditedTime { get; set; }

    [NotMapped]
    public long? RecentEditedTimeInUnix => DateTimeToUnixTime(RecentEditedTime);

    private long? DateTimeToUnixTime(DateTime? time)
    {
        if (time is null) return null;
        TimeSpan? timeSpan = time - new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)timeSpan.GetValueOrDefault().TotalMilliseconds;
    }

    private DateTime? UnixTimeToDateTime(long? unixTime)
    {
        if (unixTime is null) return null;
        DateTime localTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return localTime.AddMilliseconds((double)unixTime).ToLocalTime();
    }
}

public class AssignCareer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AssignmentId { get; set; }

    [Required]
    [EnumDataType(typeof(EducationType))]
    public EducationType EducationType { get; set; }

    [Required]
    public virtual Career Career { get; set; }

    [Column(name: "FormId")]
    public virtual int RegistrationFormId { get; set; }

    public virtual RegistrationForm RegistrationForm { get; set; }
}

public class HomeAddress
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HomeAddressId { get; set; }

    [Display(Name = "Số nhà")]
    [Required(AllowEmptyStrings = false)]
    public string Number { get; set; }

    [Display(Name = "Đường")]
    [Required(AllowEmptyStrings = false)]
    public string Street { get; set; }

    [Display(Name = "Phường")]
    [Required(AllowEmptyStrings = true)]
    public string Block { get; set; }

    [Phone]
    [Display(Name = "Số điện thoại")]
    [DataType(DataType.PhoneNumber)]
    [Required(AllowEmptyStrings = false)]
    public string PhoneNumber { get; set; }

    [Display(Name = "Mã huyện")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(2, MinimumLength = 2)]
    public virtual string DistrictId { get; set; }

    [Required]
    public virtual District District { get; set; }

    [Display(Name = "Mã tỉnh/TP")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(2, MinimumLength = 2)]
    public virtual string CityId { get; set; }

    [Required]
    public virtual City City { get; set; }
}

// Dữ liệu `Quận/Huyện` được nhập trước.
public class District
{
    [StringLength(2, MinimumLength = 2)]
    public string DistrictId { get; set; }

    public string Name { get; set; }
}

// Dữ liệu `Tỉnh/TP` được nhập trước.
public class City
{
    [StringLength(2, MinimumLength = 2)]
    public string CityId { get; set; }

    public string Name { get; set; }
}

// Dữ liệu `Ngành học` được nhập trước.
public class Career
{
    public string CareerId { get; set; }

    public string Name { get; set; }
}

public enum EducationType
{
    College = 0, University = 1
}

public enum Gender
{
    Male = 0, Femail = 1
}

public class HighSchoolResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ResultId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(3, MinimumLength = 3)]
    public string SchoolId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(2, MinimumLength = 2)]
    public virtual string CityId { get; set; }

    [Required]
    public virtual City City { get; set; }

    [Required]
    [Display(Name = "Điểm trung bình")]
    [Range(minimum: 0, maximum: 10)]
    public double AverageResult { get; set; }
}