// ------------------------------ Globals -----------------------------------------------------------
$(".homeContent").removeClass("homeContent");

$(document).ready(function () { 
    FillFlightSchedule();

    document.addEventListener('DOMContentLoaded', function () { 
      document.addEventListener('click', function (evt) {
        const a = evt.target.closest('a.schedule-link');
        if (!a) return;

        evt.preventDefault();  

        const file = a.getAttribute('data-file') || '';
        const title = a.getAttribute('data-title') || 'Schedules'; 
        WindowSettings_WithoutMenu(file, title);
      });
    });
});
 
function FillFlightSchedule() {
    const strId = new URLSearchParams(window.location.search).get("Id");

    $.ajax({
      url: '/TourDLGS/FillFlightSchedule',
      type: 'GET',
      dataType: 'json', 
      data: { strId },   
      success: function (response) {
        let res = [];

        if (Array.isArray(response)) {
          res = response;
        } else if (response && Array.isArray(response.result)) {
          res = response.result;
        } else if (response && typeof response.result === 'string') {
          try {
            res = JSON.parse(response.result);
          } catch (e) {
            console.error('Failed to parse response.result as JSON string', e);
          }
        } else {
          try {
            res = (typeof response === 'string') ? JSON.parse(response) : [];
          } catch (e) {
            console.error('Unexpected response format', e);
          }
        }
        renderFlightScheduleTable(res, strId);
      },
      error: function (xhr, status, error) {
        console.error('Error:', error);
        renderFlightScheduleTable([], strId, 'An error occurred while loading data.');
      }
    });
  }

  
function renderFlightScheduleTable(requests, strId, errorMessage) { 
  const $table = $('#grdFS');
  const $tbody = $table.find('tbody'); 
  $tbody.empty();

  const secondColHeader = (strId === "Flight")
    ? "&nbsp; Flight Name"
    : "&nbsp; Visa Information";

  const headerRowHtml = `
    <tr class="tcat" style="height:20px;">
      <th class="Headerpad" align="left" valign="middle" scope="col" style="width:8%;">&nbsp;S.No.</th>
      <th align="left" valign="middle" scope="col" style="width:50%;">${secondColHeader}</th>
    </tr>
  `;
  $tbody.append(headerRowHtml); 

  if (errorMessage) {
    $tbody.append(`
      <tr class="row1" style="height:20px;">
        <td align="left" valign="middle" style="width:8%;">-</td>
        <td align="left" valign="middle" style="width:50%;">${escapeHtml(errorMessage)}</td>
      </tr>
    `);
    return;
  }
   
  if (requests && requests.length > 0) {
    requests.forEach((item, index) => {
      const flightName = item.FLIGHTNAME || item.FlightName || item.Text || 'visa';
      const attachment = item.ATTECHMENT || item.Attachment || 'visa.pdf';
      const displayText = (strId === 'Flight') ? flightName : (item.VisaInfo || flightName);

      const filePath = `/Uploads/TourRequest/${attachment}`;

      const rowHtml = `
        <tr class="row1" style="height:20px;">
          <td align="left" valign="middle" style="width:8%;">
            <span style="padding:8px;">${index + 1}</span>
          </td>
          <td align="left" valign="middle" style="width:50%;">
            <a class="schedule-link" 
               href="${escapeHtml(filePath)}" 
               data-title="Schedules"
               style="padding:8px;">
               ${escapeHtml(displayText)}
            </a>
          </td>
        </tr>
      `;
      $tbody.append(rowHtml);
    });
  } else { 
    $tbody.append(`
      <tr class="row1" style="height:20px;">
        <td align="left" valign="middle" style="width:8%;">-</td>
        <td align="left" valign="middle" style="width:50%;">No records found.</td>
      </tr>
    `);
  }
}


function escapeHtml(str) {
  if (str == null) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
}

function WindowSettings_WithoutMenu(str1,str2)
{
	var winTop = (screen.height / 2) - 325 ;
	var winLeft = (screen.width / 2) - 425 ;
	var windowFeatures = "location=no,status=no,width=700,height=600" ;
	windowFeatures = windowFeatures + ",left = " + winLeft + "," ;
	windowFeatures = windowFeatures + "top=" + winTop + ",resizable"+ ",scrollbars" ;
	window.open(str1, str2, windowFeatures) ;
}