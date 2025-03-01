
const apiKey = 'cNNqPnwuPiecDAb0midTWtf8NJkrIXhh3qws63ns';
const addressInput = document.getElementById('AddressDeliverry');
const suggestionsContainer = document.getElementById('suggestions');
const cityInput = document.getElementById('city');
const districtInput = document.getElementById('district');
const wardInput = document.getElementById('ward');
let sessionToken = crypto.randomUUID();

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

const debouncedSearch = debounce((query) => {
    if (!query || query.length < 2) { // Nếu không có query hoặc query quá ngắn, ẩn gợi ý
        suggestionsContainer.style.display = 'none';
        return;
    }

    // Kiểm tra xem API URL có đúng không
    console.log('Fetching API with query:', query);

    fetch(`https://rsapi.goong.io/Place/AutoComplete?api_key=${apiKey}&input=${encodeURIComponent(query)}&sessiontoken=${sessionToken}`)
        .then(response => {
            // Kiểm tra status code của response
            console.log('Response status:', response.status);
            return response.json();
        })
        .then(data => {
            if (data.status === 'OK') {
                suggestionsContainer.innerHTML = '';  // Xóa nội dung cũ
                suggestionsContainer.style.display = 'block';

                // Kiểm tra xem data.predictions có dữ liệu không
                console.log('API Data:', data);

                data.predictions.forEach(prediction => {
                    const div = document.createElement('div');
                    div.className = 'suggestion-item';
                    div.textContent = prediction.description;
                    div.addEventListener('click', () => {
                        addressInput.value = prediction.description;
                        suggestionsContainer.style.display = 'none';

                        // Kiểm tra compound trước khi điền thông tin
                        if (prediction.compound) {
                            cityInput.value = prediction.compound.province || '';
                            districtInput.value = prediction.compound.district || '';
                            wardInput.value = prediction.compound.commune || '';
                        }
                    });
                    suggestionsContainer.appendChild(div);
                });
            } else {
                suggestionsContainer.style.display = 'none';
            }
        })
        .catch(error => {
            console.error('Lỗi khi gọi API:', error);
            suggestionsContainer.style.display = 'none';
        });
}, 300);

addressInput.addEventListener('input', (e) => debouncedSearch(e.target.value));

// Ẩn gợi ý khi click ra ngoài
document.addEventListener('click', function (e) {
    // Kiểm tra nếu người dùng nhấn ra ngoài input hoặc danh sách gợi ý
    if (!suggestionsContainer.contains(e.target) && e.target !== addressInput) {
        suggestionsContainer.style.display = 'none';
    }
}
