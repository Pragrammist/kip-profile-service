using System;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure;
using MongoDB.Bson;

namespace IntegrationTests;

public class ResetPasswordTestImpl
{
    readonly ResetPasswordCodeStoreImpl _resetPassword;
    readonly double _lifeTimeToTestMillisecondes;
    public ResetPasswordTestImpl()
    {
        var lifeTimeToTestMinutes = 0.1d;
        _lifeTimeToTestMillisecondes = TimeSpan.FromMinutes(lifeTimeToTestMinutes).TotalMilliseconds;
        _resetPassword = new ResetPasswordCodeStoreImpl(lifeTimeToTestMinutes);
    }

    [Fact]
    public async Task GenerateCode()
    {
        string stringSomeId = ObjectId.GenerateNewId().ToString();
        var code = await _resetPassword.GenerateCode(stringSomeId, default);
        var delayToTest = (int)_lifeTimeToTestMillisecondes * 2; 
        await Task.Delay(delayToTest);
        var codeIsValid = await _resetPassword.CodeIsValid(stringSomeId,code, default);
        codeIsValid.Should().BeFalse();
    }


    [Fact]
    public async Task CodeIsValid()
    {
        string stringSomeId = ObjectId.GenerateNewId().ToString();
        var code = await _resetPassword.GenerateCode(stringSomeId, default);
        var codeIsValid = await _resetPassword.CodeIsValid(stringSomeId,code, default);
        codeIsValid.Should().BeTrue();
    }

    [Fact]
    public async Task RegenerateCodeTest()
    {
        // пояснения.
        // здесь, чтобы проверить работу, когда пользователь повторно обновляет код,
        // корректна она или нет делается следующее:
        // сначала идут приготовления в виде высчитывания таймингов для задержки
        // первый тайминг нужен, чтобы сделать задержку перед обновлением кода,
        // будто юзер некоторое время потупил и решил заново код отправить.
        // второй тайминг нужен, чтобы проверить правильную работу таймера.
        // т.е. если таймер правильно работает при "отправке" нового кода
        // он дожен перезапуститься и 
        // старый таймер не должен удалить новый код после второй задержки
        // будто это старый код(для старого таймера время уже исстекло)
        // т.к. в первую задержку мы взяли половину времени того,
        // когда код будет удален,
        // перезапустив таймер, сгенерировав ноывй код,
        // и, сделав задержку в оставшуюся половину 
        // и, прибавив к ней часть половины
        // чтобы исключить идеального попадения тайминга в момент проверки
        // новый код должен все равно существовать, 
        // т.к. у него прошло только половина жизни, 
        // а у первого таймера жизнь все уже
        var halfTimeOfReseting = (int)(_lifeTimeToTestMillisecondes * 0.5);
        var timeToTest = (int)(_lifeTimeToTestMillisecondes - halfTimeOfReseting + halfTimeOfReseting * 0.2);
        string stringSomeId = ObjectId.GenerateNewId().ToString();
        await _resetPassword.GenerateCode(stringSomeId, default);

        await Task.Delay(halfTimeOfReseting);
        var code = await _resetPassword.GenerateCode(stringSomeId, default);

        await Task.Delay(timeToTest);
        var codeIsValid = await _resetPassword.CodeIsValid(stringSomeId, code, default);

        codeIsValid.Should().BeTrue();
    }
}

