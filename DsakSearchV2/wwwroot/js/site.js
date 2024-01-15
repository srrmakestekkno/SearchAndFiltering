// TODO: Maks antall resultater og paging
// TODO: Gi Phoung liste over de d:saker som ikke er overført til TFS ennå
const queryParameter = 'q';
let originalTitle;
let callId;

document.addEventListener("DOMContentLoaded", function () {
    originalTitle = document.title;
    const searchBox = document.getElementById('search');
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has(queryParameter)) {
        searchBox.value = urlParams.get(queryParameter).trim();
        search(searchBox, searchBox.value);
    }
    
    document.getElementById('search').onkeypress = function (e) {
        if (!e) e = window.event;
        const keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            const searchTerms = this.value.trim();
            if (searchTerms) {
                search(searchBox, searchTerms);
            }
            
            return false;
        }
    }
});

function search(searchBox, searchTerms) {
    const searchStart = new Date();
    searchBox.blur();
    const contentNode = document.getElementById('content');
    setHeader('Søker i d:sak etter');
    setSubHeader(searchTerms);
    clearSearchResults(contentNode);
    showProgressBar(contentNode);
    doSearch(searchTerms, contentNode, searchStart);
}

function clearSearchResults(contentNode) {
    const searchResultTable = document.getElementById('search-results');
    if (searchResultTable) {
        contentNode.removeChild(searchResultTable);
    }

    const searchDuration = document.getElementById('search-duration');
    if (searchDuration) {
        contentNode.removeChild(searchDuration);    
    }
}

function showProgressBar(contentNode) {
    removeProgressBar(contentNode);
    const progressDiv = document.createElement('div');
    progressDiv.classList.add('progress');
    progressDiv.id = 'progress-bar';
    const indeterminateDiv = document.createElement('div');
    indeterminateDiv.classList.add('indeterminate');
    progressDiv.appendChild(indeterminateDiv);
    contentNode.appendChild(progressDiv);
}

function removeProgressBar(contentNode) {
    const progressDiv = document.getElementById('progress-bar');
    if (progressDiv) {
        contentNode.removeChild(progressDiv);
    }    
}

function setHeader(header) {
    const searchHeader = document.getElementById('search-header');
    const textContent = document.createTextNode(header);
    searchHeader.replaceChild(textContent, searchHeader.firstChild);
}

function setSubHeader(subHeader) {
    const searchTagline = document.getElementById('search-tagline');
    const textContent = document.createTextNode(subHeader);
    searchTagline.replaceChild(textContent, searchTagline.firstChild);
}

function doSearch(searchTerms, contentNode, searchStart) {
    const xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (this.readyState == 4) {
            const contentNode = document.getElementById('content');
            if (this.status == 200) {
                const resultCallId = xmlhttp.getResponseHeader('callId');
                if (resultCallId === callId) {
                    addSearchResults(this.response, contentNode, searchStart);
                }
            } else {
                clearSearchResults(contentNode);
                removeProgressBar(contentNode);
                setHeader('Søket mot d:sak feilet');
                setSubHeader(this.status + ' ' + this.statusText);
            }
        }
    };

    xmlhttp.open('POST', '/search');
    xmlhttp.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    callId = '' + uuidv4();
    xmlhttp.setRequestHeader('callId', callId);
    const formattedJsonData = JSON.stringify(searchTerms);
    xmlhttp.send(formattedJsonData);
}

function addSearchResults(response, contentNode, searchStart) {
    clearSearchResults(contentNode);
    removeProgressBar(contentNode);

    const searchResults = JSON.parse(response);
    setHeader(searchResults.header);
    if (searchResults.query) {
        document.title = searchResults.query + ' - ' + originalTitle;
        const urlParams = new URLSearchParams(window.location.search);
        urlParams.set(queryParameter, searchResults.query);
        const queryString = '/?' + urlParams.toString();
        history.pushState({}, '', queryString);
        setSubHeader(searchResults.query);
    }
        
    if (searchResults.tickets.length > 0) {
        createResultTable(searchResults, contentNode);
    }

    writeSearchDuration(contentNode, searchStart, searchResults.numberOfTickets);
}

function createResultTable(searchResults, contentNode) {
    const tr = document.createElement('tr');
    let th = document.createElement('th');
    th.innerHTML = 'Id';
    tr.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Tittel';
    tr.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Registrert dato';
    th.classList.add('no-break');
    tr.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Kunde';
    tr.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'TFS';
    tr.appendChild(th);
    
    const resultTable = document.createElement('table');
    resultTable.classList.add('striped');
    resultTable.id = 'search-results';
    const head = document.createElement('thead');
    const body = document.createElement('tbody');
    body.id = 'result-body';
    head.appendChild(tr);
    resultTable.appendChild(head);
    resultTable.appendChild(body);
    contentNode.appendChild(resultTable);

    searchResults.tickets.forEach(function (dsak) {
        addSearchResult(body, dsak);
    });
}

function addSearchResult(tableBody, dsak) {
    const dateTimeOptions = { year: 'numeric', month: 'numeric', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    const dateTimeFormat = new Intl.DateTimeFormat('nb-NO', dateTimeOptions);
    const date = new Date(dsak.createdAt);

    let link = 'https://support.dips.no/bin/ticket.fcgi?ticketId=' + dsak.id;
    let a = document.createElement('a');
    a.setAttribute('href', link);
    a.setAttribute('target', '_blank');
    a.innerHTML = dsak.id;
    const tr = document.createElement('tr');
    let td = document.createElement('td');
    td.appendChild(a);
    tr.appendChild(td);

    td = document.createElement('td');
    td.innerHTML = dsak.title;
    tr.appendChild(td);
    
    td = document.createElement('td');
    td.innerHTML = dateTimeFormat.format(date);
    td.classList.add('no-break');
    tr.appendChild(td);

    td = document.createElement('td');
    td.innerHTML = dsak.customer;
    tr.appendChild(td);

    link = 'http://vd-tfs03:8080/tfs/DefaultCollection/DIPS/_workitems?_a=edit&id=' + dsak.tfs;
    a = document.createElement('a');
    a.setAttribute('href', link);
    a.setAttribute('target', '_blank');
    a.innerHTML = dsak.tfs;
    td = document.createElement('td');
    td.appendChild(a);
    tr.appendChild(td);

    tableBody.appendChild(tr);
}

function writeSearchDuration(contentNode, searchStart, n) {
    const duration = Math.abs(new Date() - searchStart);
    const durationInSeconds = duration / 1000;
    const roundedDuration = Math.round((durationInSeconds + 0.00001) * 100) / 100;
    const row = document.createElement('div');
    row.id = 'search-duration';
    row.classList.add('row');
    const p = document.createElement('p');
    const span = document.createElement('span');
    span.classList.add('grey-text');
    if (n === 0) {
        span.appendChild(document.createTextNode('Fant ingen d:saker (' + roundedDuration + ' sekunder)'));
    } else if (n === 1) {
        span.appendChild(document.createTextNode('Fant 1 d:sak (' + roundedDuration + ' sekunder)'));
    } else {
        span.appendChild(document.createTextNode('Fant ' + n + ' d:saker (' + roundedDuration + ' sekunder)'));
    }
    
    p.appendChild(span);
    row.appendChild(p);
    contentNode.appendChild(row);
}