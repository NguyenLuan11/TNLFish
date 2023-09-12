// Function setError() có tác dụng highlight ô input và hiển thị message error trong trường hợp validate error
function setError(ele, message) {
    let parentEle = ele.parentNode;
    parentEle.classList.add('error');
    parentEle.querySelector('small').innerText = message;
}

function setSuccess(ele) {
    ele.parentNode.classList.add('success');
}

// 2 function isEmail(), isPhone() có tác dụng kiểm tra xem đầu vào có phải là email hoặc số điện thoại hay không
function isEmail(email) {
    return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email);
}

function isPhone(number) {
    return /(84|0[3|5|7|8|9])+([0-9]{8})\b/.test(number);
}

// Hàm kiểm tra các trường input
function checkValidate() {
    // Truy cập vào các ô input
    const hotenkhEle = document.getElementById('HoTenKH');
    const tendnEle = document.getElementById('TenDN');
    const mkEle = document.getElementById('MatKhau');
    const xnmkEle = document.getElementById('XNMatKhau');
    const emailEle = document.getElementById('Email');
    const sdtEle = document.getElementById('DienThoai');
    const ngaysinhEle = document.getElementById('NgaySinh');

    // Validate dữ liệu trong các ô input và highlight
    let hotenVal = hotenkhEle.value;
    let tendnVal = tendnEle.value;
    let mkVal = mkEle.value;
    let xnmkVal = xnmkEle.value;
    let emailVal = emailEle.value;
    let sdtVal = sdtEle.value;
    let ngaysinhVal = ngaysinhEle.value;

    let isCheck = true;

    // Kiểm tra trường HoTen
    if (hotenVal == '') {
        setError(hotenkhEle, 'Họ tên không được để trống!');
        isCheck = false;
    } else {
        setSuccess(hotenkhEle);
    }

    // Kiểm tra trường TenDN
    if (tendnVal == '') {
        setError(tendnEle, 'Tên đăng nhập không được để trống!');
        isCheck = false;
    } else {
        setSuccess(tendnEle);
    }

    // Kiểm tra trường MatKhau
    if (mkVal == '') {
        setError(mkEle, 'Mật khẩu không được để trống!');
        isCheck = false;
    } else {
        setSuccess(mkEle);
    }

    // Kiểm tra trường XacNhanMatKhau
    if (xnmkVal == '') {
        setError(xnmkEle, 'Mật khẩu phải được xác nhận lại!');
        isCheck = false;
    } else if (xnmkVal != mkVal) {
        setError(xnmkEle, 'Mật khẩu xác nhận không hợp lệ!');
        isCheck = false;
    } else {
        setSuccess(xnmkEle);
    }

    // Kiểm tra trường email
    if (emailVal == '') {
        setError(emailEle, 'Email không được để trống!');
        isCheck = false;
    } else if (!isEmail(emailVal)) {
        setError(emailEle, 'Email không đúng định dạng!');
        isCheck = false;
    } else {
        setSuccess(emailEle);
    }

    // Kiểm tra trường phone
    if (sdtVal == '') {
        setError(sdtEle, 'Số điện thoại không được để trống');
        isCheck = false;
    } else if (!isPhone(sdtVal)) {
        setError(sdtEle, 'Số điện thoại không đúng định dạng');
        isCheck = false;
    } else {
        setSuccess(sdtEle);
    }

    // Kiểm tra trường NgaySinh
    if (ngaysinhVal == '') {
        setError(ngaysinhEle, 'Ngày sinh không được để trống!');
        isCheck = false;
    } else {
        setSuccess(ngaysinhEle);
    }

    return isCheck;
}

const btnRegister = document.getElementById('btn-register');
const inputEles = document.querySelectorAll('.input-row');

btnRegister.addEventListener('click', function () {
    Array.from(inputEles).map((ele) =>
        ele.classList.remove('success', 'error')
    );
    let isValid = checkValidate();

    if (isValid) {
        alert('Gửi đăng ký thành công');
    }
});