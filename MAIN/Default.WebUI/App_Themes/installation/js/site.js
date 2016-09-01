String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};

function isValid(str) { return /^[\w\s]+$/.test(str); }
function isValidMail(mail) { return /^[\w.\-_0-9]+@[\w\-_]+\.[\A-Za-z]+$/.test(mail); }
function isValidPassword(pw) {
    var validLength = pw.length >= 6 && pw.length <= 12;
    var hasBigLetter = /[A-Z]+/.test(pw);
    var hasSmallLetter = /[a-z]+/.test(pw);
    var hasNumber = /[0-9]+/.test(pw);
    return validLength && hasBigLetter && hasSmallLetter && hasNumber;
}