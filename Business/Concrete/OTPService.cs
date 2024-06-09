using Business.Abstract;
using Business.MessageBroker;
using Core.CrossCuttingConcerns;
using Core.Exceptions;
using Core.Utils.Auth;
using Core.Utils.Mail;
using DataAccess.Abstract;
using Model.Dtos.UserDtos;
using Model.Entities;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class OTPService : IOTPService
{
    private readonly IOTPDal _OTPDal;  
    private readonly MailMessageBrokerProducer _mailMQProducerService;  
    public OTPService(IOTPDal OTPDal ,MailMessageBrokerProducer mailMQProducerService)
    {
        _OTPDal = OTPDal;
        _mailMQProducerService = mailMQProducerService;
    }


    public async Task SendConfirmationOTP(User user)
    {
        OTP otp = new OTP()
        {
            UserId = user.Id,
            Code = OTPHelper.GenerateSecureVerificationCode(),
            ExpiryTime = DateTime.UtcNow.AddMinutes(5)
        };
        bool isAlreadyExist = await _OTPDal.IsExistAsync(filter: o => o.UserId == user.Id);
        if (!isAlreadyExist) await _OTPDal.AddAsync(otp);
        if (isAlreadyExist) await _OTPDal.UpdateAsync(otp);

        MailSendModel mailModel = new MailSendModel()
        {
            Subject = "OneDay, Verification  Code.",
            HtmlContent = $"welcome you to OneDay! please use the verification code below <hr> Verification Code: <b style='color:#0556f3;'>{otp.Code}</b> <hr> <p>Please enter this code on the verification page <br> <span style='font-size:x-small;'> Code expiration date <br> (UTC) {otp.ExpiryTime.ToString("dd.MM.yyyy HH:mm")} </span> </p>>",
            RecipientEmailList = new List<string>() { user.Email! },
        };

        _mailMQProducerService.SendByDirectExc<MailSendModel>(mailModel);
    }

    public async Task VerifyConfirmationOTP(OtpControlDto otpControlDto)
    {
        OTP storedOTP = await _OTPDal.GetAsync(filter: o => o.UserId == otpControlDto.UserId);
        if (storedOTP == null) throw new BusinessException("Not Exist Any Code in System");

        if (otpControlDto.Code != storedOTP.Code) throw new BusinessException("Code is not correct");
        if (DateTime.UtcNow.AddMinutes(1) >= storedOTP.ExpiryTime) throw new BusinessException("Expiration Time Over");
    }
}
