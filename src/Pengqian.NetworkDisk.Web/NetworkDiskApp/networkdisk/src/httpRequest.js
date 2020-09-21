import axios from 'axios';

// axios 响应拦截器
axios.interceptors.response.use(function (response) {
    return Promise.resolve(response);
}, function (error) {
    if (error.response.status) {
        switch (error.response.status) {
            case 403:
                break;
            case 500:
                console.info(error.response.data);
                break;
            default:
                console.info(error.response.data);
                break;
        }
    }
    console.info("response error interceptors end.");
    return Promise.reject(error);
});

axios.interceptors.request.use(x => {
    if(typeof(localStorage.token) !== "undefined"){
        x.headers.Authorization = "Bearer " + localStorage.token;    
    }
    return x;
});

export function fetch(url, data = {}) {
    ///<summary>ajax get request</summary>
    ///<param name="url">请求地址</param>
    ///<param name="data">请求数据</param>
    return new Promise((resolve, reject) => {
        axios.get(url, { params: data }).then(function (response) {
            resolve(response.data);
        }).catch(error => reject(error));
    });
}

export function post(url, data = {}) {
    ///<summary>ajax get request</summary>
    ///<param name="url">请求地址</param>
    ///<param name="data">请求数据</param>
    return new Promise((resolve, reject) => {
        axios.post(url, data).then(function (response) {
            resolve(response.data);
        }).catch(error => reject(error));
    });
}