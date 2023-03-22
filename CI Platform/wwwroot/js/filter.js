// recomand user
function recomand(Email, MI) {

    //alert(MI);
    $.ajax({
                url: '/User/RecomandUser',
        type: 'POST',
        data: { EmailId: Email, MissionId: MI },
                success: function (result) {

                    alert("send");

                }
            });
}








var debounceTimer;

function debounce(func, delay) {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(func, delay);
}


document.getElementById("search-bar").addEventListener("input", function () {
    debounce(function () {
        search(document.getElementById("search-bar").value);
    }, 1500); // adjust the delay time as needed
});

function search(query) {
    // Get the current URL
    let url = window.location.href;

    let separator = url.indexOf('?') !== -1 ? '&' : '?';

    // Check if the searchQuery parameter already exists in the URL
    if (url.includes('searchQuery=')) {
        // Replace the value of the searchQuery parameter
        url = url.replace(/searchQuery=([^&]*)/, 'searchQuery=' + query);
    } else {
        // Append the parameter to the URL
        url += separator + 'searchQuery=' + query;
    }

    // Navigate to the updated URL
    window.location.href = url;


}

function updateUrlForCountries() {

    let url = window.location.href;

    let separator = url.indexOf('?') !== -1 ? '&' : '?';

    const checkedIds = Array.from(document.querySelectorAll('input[name="countries"]:checked')).map(el => el.id);
    /* const queryString = new URLSearchParams({ countries: checkedIds });*/

    url += separator + 'FCountries=' + checkedIds;
    window.location.href = url;
    /* window.history.replaceState(null, null, '?' + queryString.toString());*/
}

function updateUrlForSortby() {
    var c = document.getElementById("ci");
    c.style.display = "none";
    let url = window.location.href;
    c = "";
    url = url.replace(/ACities=([^&]*)/, 'ACities=' + c);
    window.location.href = url;
}


function updateUrlForCities() {
    let url = window.location.href;

    let separator = url.indexOf('?') !== -1 ? '&' : '?';

    const checkedIds = Array.from(document.querySelectorAll('input[name="cities"]:checked')).map(el => el.id);
    /* const queryString = new URLSearchParams({ countries: checkedIds });*/

    url += separator + 'FCities=' + checkedIds;
    window.location.href = url;
    /* window.history.replaceState(null, null, '?' + queryString.toString());*/
}

function updateUrlForThemes() {
    let url = window.location.href;

    let separator = url.indexOf('?') !== -1 ? '&' : '?';

    const checkedIds = Array.from(document.querySelectorAll('input[name="themes"]:checked')).map(el => el.id);
    /* const queryString = new URLSearchParams({ countries: checkedIds });*/

    url += separator + 'FThemes=' + checkedIds;
    window.location.href = url;
    /* window.history.replaceState(null, null, '?' + queryString.toString());*/
}


// Get the filters section element
const filtersSection = document.querySelector('.filters-section');

// Get the clear button element
const clearButton = document.getElementById('ClearBtn');

// Check if there are any child elements in the filters section except for the clear button
const hasChildElements = Array.from(filtersSection.children).some(element => element !== clearButton);

// If there are no child elements except for the clear button, hide the clear button
if (!hasChildElements) {
    clearButton.style.display = 'none';
} else {
    clearButton.style.display = 'inline-block'; // Or whatever your initial display value was
}

function removeUrlParams() {
    filtersSection.classList.add('d-none');
    window.history.replaceState({}, '', window.location.pathname);
    location.reload();

}


function removeCountryFilter(id) {

    let url = window.location.href;
    const urlParts = url.split('?');

    if (urlParts.length >= 2) {
        const baseUrl = urlParts.shift();
        const queryString = urlParts.join('?');
        const params = new URLSearchParams(queryString);

        const countries = params.getAll('FCountries').filter(country => country !== id);

        if (countries.length > 0) {
            params.delete('FCountries');
            countries.forEach(country => params.append('FCountries', country));
        } else {
            params.delete('FCountries');
        }

        const updatedParams = params.toString();
        if (updatedParams) {

            url = `${baseUrl}?${updatedParams}`;
        } else {
            url = baseUrl;
        }

        window.location.href = url;
    }

}



function removeCityFilter(id) {
    let url = window.location.href;
    const urlParts = url.split('?');

    if (urlParts.length >= 2) {
        const baseUrl = urlParts.shift();
        const queryString = urlParts.join('?');
        const params = new URLSearchParams(queryString);

        const countries = params.getAll('FCities').filter(country => country !== id);

        if (countries.length > 0) {
            params.delete('FCities');
            countries.forEach(country => params.append('FCities', country));
        } else {
            params.delete('FCities');
        }

        const updatedParams = params.toString();
        if (updatedParams) {
            url = `${baseUrl}?${updatedParams}`;
        } else {
            url = baseUrl;
        }

        window.location.href = url;
    }
}

function removeThemeFilter(id) {
    let url = window.location.href;
    const urlParts = url.split('?');

    if (urlParts.length >= 2) {
        const baseUrl = urlParts.shift();
        const queryString = urlParts.join('?');
        const params = new URLSearchParams(queryString);

        const countries = params.getAll('FThemes').filter(country => country !== id);

        if (countries.length > 0) {
            params.delete('FThemes');
            countries.forEach(country => params.append('FThemes', country));
        } else {
            params.delete('FThemes');
        }

        const updatedParams = params.toString();
        if (updatedParams) {
            url = `${baseUrl}?${updatedParams}`;
        } else {
            url = baseUrl;
        }

        window.location.href = url;
    }
}


//Mission Details js

let slideIndex = 1;
showSlides(slideIndex);

// Next/previous controls
function plusSlides(n) {
    showSlides(slideIndex += n);
}

// Thumbnail image controls
function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    let dots = document.getElementsByClassName("demo");
    let captionText = document.getElementById("caption");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
    captionText.innerHTML = dots[slideIndex - 1].alt;
}


// opensinglemission()

function opensinglemission() {
    let url = "/Home/SingleMission";

    let separator = url.indexOf('?') !== -1 ? '&' : '?';
    var res = $('#missionid').html();
    console.log(res);
    //let checkedIds = document.getElementById("missionid").;
    /* const queryString = new URLSearchParams({ countries: checkedIds });*/

    url += separator + 'MissionId=' + res;
    window.location.href = url;
}