function AxAfterTstLoad() {
    if(transid == "testb"){
		btn18onclick = function(){

			var tbldiv = document.createElement("div");
			var tblhead = document.createElement("h3");
			tblhead.classList.add("mb-3");
			tblhead.innerHTML = "Customer Details";
			tbldiv.appendChild(tblhead);

			var modal_id = "mod_UpdateTask";
			
			$.ajax({
				url: top.window.location.href.toLowerCase().substring(0, top.window.location.href.toLowerCase().indexOf("/aspx/")) + '/CustomWebservice.asmx/GetShopifyClientDetails',
				type: 'POST',
				cache: false,
				async: true,
				dataType: 'json',
				contentType: "application/json",
				success: function (data) {
					var jsondata = JSON.parse(data.d).customers;

					var table = document.createElement('table');
					table.classList.add("table", "table-bordered");

					// Create table header
					var thead = table.createTHead();
					var headerRow = thead.insertRow();
					var headings = ['First Name', 'Last Name', 'Email', 'Phone Number'];

					headings.forEach(function (heading) {
						var th = document.createElement('th');
						th.textContent = heading;
						headerRow.appendChild(th);
					});

					// Create table body
					var tbody = table.createTBody();

					// Loop through JSON data to create table rows
					jsondata.forEach(function (customer) {
						var row = tbody.insertRow();
						var data = [customer.first_name, customer.last_name, customer.email, customer.phone];

						data.forEach(function (value) {
							var cell = row.insertCell();
							cell.textContent = value;
						});
					});

					tbldiv.appendChild(table);

					let popupHtml = tbldiv.outerHTML;

					let myModal = new BSModal(modal_id, "", popupHtml,
						(opening) => { }, (closing) => { }
					);

					myModal.changeSize("xl");
					myModal.hideHeader();
					myModal.hideFooter();
					myModal.showFloatingClose();
				},
				error: function (error) {
					showAlertDialog("error", error);
				}
			});
			
		}
		
		btn17onclick = function(){
			
			$.ajax({
				url: top.window.location.href.toLowerCase().substring(0, top.window.location.href.toLowerCase().indexOf("/aspx/")) + '/CustomWebservice.asmx/GetShopifyCountDetails',
				type: 'POST',
				cache: false,
				async: true,
				dataType: 'json',
				contentType: "application/json",
				success: function (data) {
					var count = JSON.parse(data.d).count;
					UpdateFieldArray(`customercnt000F1`, "0", count);
				   document.querySelector("#customercnt000F1").value = count;

				},
				error: function (error) {
				}
			});
		}
	}
}
