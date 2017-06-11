@model object.Web.Models.ViewModels.ItemViewModel<int?>

@{
    ViewBag.Title = "Edit";
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";

}


@section style {

    <!-- Include the plugin's CSS and JS: -->

    <link rel="stylesheet" href="~/Scripts/multiselect/css/bootstrap-multiselect.css" type="text/css" />

    <style>
        .btn-group.open > .dropdown-toggle.btn, .btn-group.open > .dropdown-toggle.btn.btn-default,
        .btn-group-vertical.open > .dropdown-toggle.btn,
        .btn-group-vertical.open > .dropdown-toggle.btn.btn-default {
            background-color: #212121 !important;
        }
    </style>

}


<div class="col-lg-12">
    <div class="card">
        <form method="get" action="/" class="form-horizontal" id="insert">
            <div class="card-header card-header-text" data-background-color="rose">
                <h4 class="card-title" id="title">Create FAQs</h4>
            </div>



            <div class="form-group targetSelect">
                <label for="catList" class="col-sm-2 label-on-left">Category:</label>

                <select id="catList" multiple="multiple" name="catList">
                    <option value="">Select</option>
                </select>

            </div>

            <div class="form-group">
                <label for="question" class="col-sm-2 label-on-left">Question</label>
                <div class="col-sm-10">
                    <div class="form-group label-floating is-empty">
                        <label class="control-label"></label>

                        <textarea name="question" id="question" rows="3" cols="10" class="form-control" placeholder="Question"></textarea>
                        <span class="help-block"></span>
                        <span class="material-input"></span>
                    </div>
                </div>

            </div>

            <div class="form-group">
                <label for="answer" class="col-sm-2 label-on-left">Answer</label>
                <div class="col-sm-10">
                    <div class="form-group label-floating is-empty">
                        <label class="control-label"></label>
                        <textarea name="answer" id="answer" rows="3" cols="10" class="form-control" placeholder="Answer"></textarea>

                        <span class="help-block"></span>
                        <span class="material-input"></span>
                    </div>
                </div>

            </div>
            <div class="form-group">
                <label for="displayOrder" class="col-sm-2 label-on-left">Display Order</label>
                <div class="col-sm-10">
                    <div class="form-group label-floating is-empty">
                        <label class="control-label"></label>
                        <input type="text" name="displayOrder" class="form-control" id="displayOrder" placeholder="Display Order">

                        <span class="help-block"></span>
                        <span class="material-input"></span>
                    </div>
                </div>

            </div>


            <hr />
            <div class="text-center">

                <button type="button" id="btnSubmit" class="btn btn-primary">Submit</button>

            </div>

        </form>

        <div class="container">

            <input type="hidden" id="faqId" value="@Model.Item" />

        </div>

    </div>
</div>


@section scripts
{
    <script type="text/javascript" src="~/Scripts/multiselect/js/bootstrap-multiselect.js"></script>
    <script type="text/javascript" src="~/Scripts/object.services.faqs.js"></script>
    <script type="text/javascript">

		object.page.faqId = null;

		console.log(object.page.faqId);

		object.page.startUp = function () {

			console.log("I am currently executing the page start up");
			object.page.initializeValidation();
			object.page.faqId = $("#faqId").val();




			object.services.faqs.getfaqcategory(object.page.handlers.catReceived, object.page.handlers.errorReceivedFaq);
			$("#btnSubmit").on("click", object.page.handlers.insert);

			if (object.page.faqId > 0) {
                		console.log(object.page.faqId);


				$("#btnSubmit").on("click", object.page.handlers.insert);

				object.services.faqs.btnFaqEdit(object.page.faqId, object.page.handlers.contentEditFaq, object.page.handlers.errorReceivedFaq);
				$("#btnSubmit").html("Edit");
				$("#title").text("Edit FAQ");
			}
		};

		//Form VALIDATION
		object.page.initializeValidation = function () {
			jQuery.validator.setDefaults({
				debug: true
			})
			$("#insert").validate(
				{
					rules:
					   {
						   "catList": "required",
						   "question": {
							   required: true
								  , maxlength: 200

						   }
				   , "answer":
					   {
						   required: true
						   , maxlength: 1000

					   }
				   , "displayOrder":
						{
							required: true
						  , number: true
                            , noZero: true
						}

					   }
					, messages:
		   {
			   "displayOrder":
			   {
			       number: "The input must only contain numbers"
                   , noZero: "Display order can't be zero"
			   }
			   , "question": {
				   maxlength: "The input can't be more than 200 characters"

			   }

			   , "answer": {
				   maxlength: "The input can't be more than 1000 characters"

			   }
		   },

					//errorPlacement: function (error, element) {
					//	$(element).parent('div').addClass('has-error');
					//}

				})
		};

		jQuery.validator.addMethod("noZero", function (value, element)
		{ return /^[1-9]+$/.test(value); });

		object.page.handlers.catReceived = function (data) {
			console.log(data);


			for (var i = 0; i < data.items.length; i++) {
				var optionTemplate = $("<option></option>");

				optionTemplate.val(data.items[i].id);
				optionTemplate.text(data.items[i].title);

				$('#catList').append(optionTemplate);

			}
			$('#catList').multiselect();


		};

		object.page.handlers.contentEditFaq = function (data, status, xhr) {
			console.log(data);
			console.log("Append");
			var catId = null;
			var cats = [];
			if (data.item) {
				// Post FAQs to template

				var currentCategory = data.item.categories;
				console.log(currentCategory);

				for (var j = 0; j < currentCategory.length; j++) {
					// Post to template, call function to display text of category
					catId = data.item.categories[j].id;
					cats.push(" " + data.item.categories[j].title);
				}

			};

			var question = data.item.question;
			var answer = data.item.answer;
			var displayOrder = data.item.displayOrder;
			var createdDate = data.item.createdDate;
			var modifiedDate = data.item.modifiedDate;
			var createdBy = data.item.createdBy;
			var modifiedBy = data.item.modifiedBy;
			var id = data.item.id;

			$("#question").val(question);
			$("#answer").val(answer);
			$("#displayOrder").val(displayOrder);


		};

		object.page.handlers.insert = function () {

			if ($("#insert").valid() && object.page.faqId == 0) {
				var data = object.page.readFaq();


				object.services.faqs.faqPost(data, object.page.handlers.contentReceivedFaq, object.page.handlers.errorReceivedFaq);
			}

			else if ($("#insert").valid() && object.page.faqId > 0) {
				console.log("Edit");
				var data = object.page.readFaq();

				object.services.faqs.faqUpdate(data, object.page.faqId, object.page.handlers.updateReceivedFaq, object.page.handlers.errorReceivedFaq);

			}

		};

		object.page.handlers.contentReceivedFaq = function (data, status, xhr) {
			console.log(data + "/" + status)
			var id = data.item;
			console.log(id);

			if (data.item > 0) {
			    window.location = "/admin/legacy/FAQs/list/";

			}
			else {
				alert(xhr.responseText);

			}
		};


		object.page.handlers.updateReceivedFaq = function (data, status, xhr) {
				console.log(data);
				$("form")[0].reset();
				window.location = "/admin/legacy/FAQs/list/";

		};
		object.page.handlers.errorReceivedFaq = function (xhr, status, errorThrown) {
			console.log(status + "/" + errorThrown)
			console.log(xhr);
			alert(xhr.responseText);
		};

		object.page.readFaq = function () {

			var data = {
				question: $("#question").val(),
				answer: $("#answer").val(),
				displayOrder: $("#displayOrder").val(),
				categories: $("#catList").val(),
			};
			return data;
		};

    </script>
}
