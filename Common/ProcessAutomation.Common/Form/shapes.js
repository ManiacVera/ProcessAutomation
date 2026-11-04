var canvasState = null;

window.onload = function() {
	document.addEventListener('keydown', function(e){
		if (canvasState){
			const ctrlKey = 17, cmdKey = 91, vKey = 86, cKey = 67, suprKey = 46, insKey = 45, zKey = 90, yKey = 89, sKey = 83;
						
			if ((event.ctrlKey && event.keyCode == sKey)
				|| (event.cmdKey && event.keyCode == sKey)) 
			{
				canvasState.Save();
			}
			else if ((event.ctrlKey && event.keyCode == cKey)
				|| (event.cmdKey && event.keyCode == cKey))
			{
				canvasState.Copy();
			}
			else if ((event.ctrlKey && event.keyCode == vKey)
				|| (event.cmdKey && event.keyCode == vKey)) 
			{
				canvasState.Paste();
			}
			else if (event.keyCode == insKey) 
			{
				canvasState.Insert();
			}
			else if (event.keyCode == suprKey) 
			{
				canvasState.Delete();
			}
			else if ((event.ctrlKey && event.keyCode == zKey)
				|| (event.cmdKey && event.keyCode == zKey))
			{
				canvasState.Undo();
			}
			else if ((event.ctrlKey && event.keyCode == yKey)
				|| (event.cmdKey && event.keyCode == yKey))
			{
				canvasState.Redo();
			}
			
			e.preventDefault();
		}
  }, false);

}

function UndoRedoManager(){
	this.stack = {undo: [], redo: []};	
}

const UndoRedoActionEnum = {"ADD": 1, "UPDATE": 2, "RESTORE": 3, "DELETE": 4, "SELECT": 5, "UNSELECT": 6}

UndoRedoManager.prototype.Init = function(){
	if (!this.stack){
		this.stack = {undo: [], redo: []};
	}
	
	if (!this.stack.undo){
		this.stack.undo = [];
	}
	
	if (!this.stack.redo){
		this.stack.redo = [];
	}
}

UndoRedoManager.prototype.AddObject = function(action, object, inverseObject){
	this.Init();
	
	var firstObjectValue = null;
	var firstInverseObjectValue = null;
	
	if (Array.isArray(object))
		firstObjectValue = object[0];
	else
		firstObjectValue = object;
	
	if (Array.isArray(inverseObject))
		firstInverseObjectValue = inverseObject[0];
	else
		firstInverseObjectValue = inverseObject;
	
	// LUIS VERA: CUANDO UN SHAPE CONTIENE VARIAS LINEAS, ESTAS AUNQUE SEAN LAS MISMAS, LLEGAN EN DIFERENTE ORDEN EN LOS OBJETOS DE ENTRADA
	// Y ESO HACE QUE LA VALIDACIÓN NO SE CUMPLA, DE MOMENTO DE VA A VALIDAR EL PRIMER OBJETO
	if (action && object 
		&& (!Array.isArray(object) || object[0])
		&& (action != UndoRedoActionEnum.UPDATE || JSON.stringify(firstObjectValue) !== JSON.stringify(firstInverseObjectValue)))
		// && (action != UndoRedoActionEnum.UPDATE || JSON.stringify(object) !== JSON.stringify(inverseObject)))
		{
			
		var inverse = null;
		var undoObject = null;

		if (this.stack.undo.length > 0){
			undoObject = this.stack.undo[this.stack.undo.length];
		}
		
		if (undoObject == null || (undoObject.object != object && undoObject.action != action && undoObject.inverseObject != inverseObject))
		{
			switch(action){
				case UndoRedoActionEnum.ADD:
					inverse = UndoRedoActionEnum.DELETE;
					break;
				case UndoRedoActionEnum.DELETE:
					inverse = UndoRedoActionEnum.ADD;
					break;
				case UndoRedoActionEnum.RESTORE:
					inverse = UndoRedoActionEnum.UPDATE;
					break;
				case UndoRedoActionEnum.UPDATE:
					inverse = UndoRedoActionEnum.RESTORE;
					break;
				case UndoRedoActionEnum.SELECT:
					inverse = UndoRedoActionEnum.UNSELECT;
					break;
				case UndoRedoActionEnum.UNSELECT:
					inverse = UndoRedoActionEnum.SELECT;
					break;
			}
			
			this.stack.undo.push({action: action, inverse: inverse, object: object, inverseObject: inverseObject});
			this.stack.redo = [];
		}
	}
}

UndoRedoManager.prototype.Undo = function(fn){
	this.Init();
	
	var element = null;	
	if (this.stack.undo.length > 0){
		element = this.stack.undo.pop();		
	}
	
	if (element){
		this.stack.redo.push({action: element.inverse, inverse: element.action, object: element.object, inverseObject: element.inverseObject});		
		this.ProcessUndo(element, fn);
	}
}

UndoRedoManager.prototype.ProcessUndo = function(element, fn){
	if (element && element.object){		
		if (fn){
			fn(element.inverse, element.object);
		}
	}
}

UndoRedoManager.prototype.Redo = function(fn){
	this.Init();
	
	var element = null;
	if (this.stack.redo.length > 0){		
		element = this.stack.redo.pop();
	}
	
	if (element){		
		this.stack.undo.push({action: element.inverse, inverse: element.action, object: element.object, inverseObject: element.inverseObject});
		this.ProcessRedo(element, fn);		
	}
}

UndoRedoManager.prototype.ProcessRedo = function(element, fn){
	if (element && element.object){		
		if (fn){
			fn(element.inverse, element.inverseObject || element.object);
		}
	}
}

const DrawSelector = (() => {
    var x1, y1, x2, y2;
    var show = false;
	
    function Fix() {
        rect.x = Math.min(x1, x2);
        rect.y = Math.min(y1, y2);
        rect.w = Math.max(x1, x2) - Math.min(x1, x2);
        rect.h = Math.max(y1, y2) - Math.min(y1, y2);
    }
    
	function Draw(ctx) { 
		ctx.setLineDash([6]);
		ctx.lineWidth = 1; 
		
		ctx.strokeStyle = 'rgba(0,0,0,1)';
		ctx.strokeRect(this.x, this.y, this.w, this.h); 		
		
		ctx.beginPath();
		ctx.fillStyle = 'rgba(0,0,0,0.7)';
		ctx.rect(this.x, this.y, this.w, this.h);
		ctx.globalAlpha = 0.2;
		ctx.fill();
		
		ctx.setLineDash([0]);
		ctx.globalAlpha = 1;
	}
	
    const rect = {x : 0, y : 0, w : 0, h : 0, Draw};
	
    const drawSelectorResult = {
        Restart(point) {
            x2 = x1 = point.x;
            y2 = y1 = point.y;
            Fix();
            show = true;
        },
        Update(point) {
            x2 = point.x;
            y2 = point.y;
            Fix();
            show = true;
        },
        ToRect() {
            show = false;
            return Object.assign({}, rect);
        },
        Draw(ctx) {
            if (show) { rect.Draw(ctx) }
        },
		ContainsPoint(x, y){
			var result = false;			
			
			if ((rect.x <= x && rect.x + rect.w >= x)
				&& (rect.y <= y && rect.y + rect.h >= y)){
				result = true;
			}else{
				result = false;
			}
			  
			return result;
		},
		ContainsRect(x, y, w, h) {
			var result = false;
			
			let points =  [ 
				{ x: x, y: y},
				{ x: w + x, y: y},
				{ x: w + x, y: h + y},
				{ x: x, y: h + y}
			];
			
			for(let countPoint = 0; countPoint < points.length; countPoint++){		
				if ((rect.x <= points[countPoint].x) && (rect.x + rect.w >= points[countPoint].x)
					&& (rect.y <= points[countPoint].y) && (rect.y + rect.h >= points[countPoint].y)){
					result = true;
				}else{
					result = false;
					break;
				}
			}
			  
			return result;
		},		
        show : false,
    }
		
    return drawSelectorResult;
})();

const ShapeTypeEnum = {
	POLYGON: 1,
	SQUARE: 2,
	RECTANGLE: 3,
	TRIANGLE : 4,
	LINE: 5,
	CIRCLE: 6,
	RHOMBUS: 7,	
	[1]: 'POLYGON',
	[2]: 'SQUARE',
	[3]: 'RECTANGLE',
	[4]: 'TRIANGLE',
	[5]: 'LINE',
	[6]: 'CIRCLE',
	[7]: 'RHOMBUS'
}

const UrlToBase64 = url => fetch(url)
	.then(response => response.blob())
	.then(blob => new Promise((resolve, reject) => {
		const reader = new FileReader()
		reader.onloadend = () => resolve(reader.result)
		reader.onerror = reject
		reader.readAsDataURL(blob)
  }));

function ShapeService() {
}

ShapeService.GetCenterRectangle = function(x, y, w, h){	
	var center = {x: x + w/2, y: y + h/2};	
	return center;
}

ShapeService.GetMinCoordinate = function(object){
	let minValue = null;
	
	if (object){		
		if (Array.isArray(object)){		
			minValue = object.reduce(function (x, y) {
				return Math.min(x, y);
			});
		}else{
			minValue = Math.min(object.x, object.y);
		}
	}
	
	return minValue;
}

ShapeService.GetMaxCoordinate = function(object){
	let minValue = null;
	
	if (object){		
		if (Array.isArray(object)){		
			minValue = object.reduce(function (x, y) {
				return Math.max(x, y);
			});
		}else{
			minValue = Math.max(object.x, object.y);
		}
	}
	
	return minValue;
}

ShapeService.FileToBase64 = function(file, callback){
	var xhr = new XMLHttpRequest();       
    xhr.open("GET", file, true); 
    xhr.responseType = "blob";
	xhr.onload = function (e) {
		var reader = new FileReader();
		reader.onload = function(event) {
		   var res = event.target.result;
		}
		
		var result = this.response;		
		if (callback){
			callback(reader.readAsDataURL(result));
		}
    };
    xhr.send();
}

ShapeService.toDataURL = function(src, callback, outputFormat) {
  var img = new Image();
  img.crossOrigin = 'Anonymous';
  img.onload = function() {
    var canvas = document.createElement('CANVAS');
    var ctx = canvas.getContext('2d');
    var dataURL;
    canvas.height = this.naturalHeight;
    canvas.width = this.naturalWidth;
    ctx.drawImage(this, 0, 0);
    dataURL = canvas.toDataURL(outputFormat);
	
	if (callback){
		callback(dataURL);
	}
  };
  img.src = src;
  if (img.complete || img.complete === undefined) {
    img.src = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
    img.src = src;
  }
}

ShapeService.LoadImage = function(imageSource, dx, dy, dw, dh, sx, sy, sw, sh, fn){	
	var imgButton = null;
	if (imageSource && (sh || dh || dy)){
		imgButton = new Image();
		imgButton.src = imageSource;
		
		imgButton.onload = function(){
			if (fn){
				fn(imgButton);
			}
		}
	}
	return imgButton;
}

ShapeService.GetUniqueID = function(length){
	length = length || 16;
	return parseInt(Math.ceil(Math.random() * Date.now()).toPrecision(length).toString().replace(".", ""));
}

ShapeService.DirectionToVector = function(direction){
	var vector = null;
	
	switch (direction) {
		case "W":
			vector = {x:-1,y:0};
			 break;
		case "E":
			vector = {x:1,y:0};
			break;
		case "S":
			vector = {x:0,y:1};
			break;
		case "N":
			vector = {x:0,y:-1};
			break;
		case "NE":
			vector = {x:1,y:-1};
			break;
		case "NW":
			vector = {x:-1,y:-1};
			break;
		case "SE":
			vector = {x:1,y:1};
			break;
		case "SW":
			vector = {x:-1,y:1};
			break;
	}
		
	return vector;
}

ShapeService.PointInPolygon = function (n, vertices, touch) {
	var i, j, c = false;
	for (i = 0, j = n - 1; i < n; j = i++) {
		if (((vertices[i].y > touch.y) != (vertices[j].y > touch.y)) &&
			(touch.x < (vertices[j].x - vertices[i].x) * (touch.y - vertices[i].y) / (vertices[j].y - vertices[i].y) + vertices[i].x )) {
			c = !c;
		}
	}
	return c;
}

ShapeService.ContainsPointInRect = function(rect, point) {
	var result = false;
	
	if (rect && point){
		
		result = (rect.x <= point.x) && (rect.x + rect.w >= point.x) &&
			  (rect.y <= point.y) && (rect.y + rect.h >= point.y);
	}
	  
	return result;
}

ShapeService.DistanceCircle = function(x1, y1, x2, y2){
	let xDistance = x2 - x1;
	let yDistance = y2 - y1;
	return Math.sqrt(Math.pow(xDistance, 2) + Math.pow(yDistance, 2));
}
	
ShapeService.GetCadinalDirection = function(angleDegrees) 
{
	var angle = angleDegrees;
    var directions = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
    var index = Math.round(((angle %= 360) < 0 ? angle + 360 : angle) / 45) % 8;
    return directions[index];
}
	
ShapeService.GetRadianAngle = function(x1, y1, x2, y2){
	var dx = x2-x1; // deltaX used in length and angle calculations 
	var dy = y2-y1; // deltaY used in length and angle calculations
	
	return Math.atan2(dy,dx);
}

ShapeService.GetAngle = function(x1, y1, x2, y2){
	return (ShapeService.GetRadianAngle(x1, x2, y1, y2)) * 180 / Math.PI;
}
	
ShapeService.GetLineVertices = function(x1, y1, x2, y2, lineWidth) {
	var lineRadianAngle = ShapeService.GetRadianAngle(x1, y1, x2, y2);
	var lineWidthProportional = lineWidth;
	
	var vertices = [
		{ x: x1 - lineWidthProportional, y: y1 - lineWidthProportional },
		{ x: x1 + lineWidthProportional * Math.cos(lineRadianAngle), y: y1 + lineWidthProportional * Math.sin(lineRadianAngle)},
		{ x: x2 + lineWidthProportional * Math.cos(lineRadianAngle), y: y2 + lineWidthProportional * Math.sin(lineRadianAngle)},
		{ x: x2 - lineWidthProportional, y: y2 - lineWidthProportional }
	];
	
	return vertices;
}

ShapeService.GetDistanceLine = function(x1, y1, x2, y2){
	let xDistance = x2 - x1;
	let yDistance = y2 - y1;
	return Math.sqrt(Math.pow(xDistance, 2) + Math.pow(yDistance, 2));
}

ShapeService.SplitLine = function(x1, y1, x2, y2) {
	var lineRadianAngle = ShapeService.GetRadianAngle(x1, y1, x2, y2);
	var middleDistance = ShapeService.GetDistanceLine(x1, y1, x2, y2) / 2;
	
	var vertices = [
		{ x: x1, y: y1 },
		{ x: x1 + middleDistance * Math.cos(lineRadianAngle), y: y1 + middleDistance * Math.sin(lineRadianAngle)},
		{ x: x2, y: y2}
	];
	
	return vertices;
}

ShapeService.IsRightAngle = function(x1, y1, x2, y2){
	var isRectAngle = false;	
	if (ShapeService.GetRadianAngle(x1, y1, x2, y2) % (Math.PI/2) == 0){	
		isRectAngle = true;		
	}	
	return isRectAngle;
}

ShapeService.IsRightAngleBetweenLines = function(lines){
	var isRectAngle = false;
	
	if (lines && lines.length == 2){
		var dAx = lines[0].x2 - lines[0].x1;
		var dAy = lines[0].y2 - lines[0].y1;
		var dBx = lines[1].x2 - lines[1].x1;
		var dBy = lines[1].y2 - lines[1].y1;
		var angle = Math.atan2(dAx * dBy - dAy * dBx, dAx * dBx + dAy * dBy);
		if(angle < 0) {angle = angle * -1;}
		var degree_angle = angle * (180 / Math.PI);
		
		if ((degree_angle % 90) == 0){
			isRectAngle = true;
			console.log("si es angulo recto");
		}
	}
	
	return isRectAngle;
}

ShapeService.LineToSegments = function(x1, y1, x2, y2, bd, ed, ctx){
	var resultLine = [];
	var splLine = [{x: x1, y: y1}, {x: x2, y: y2}];
	var lines = null;
	if (splLine.length > 1){
		var intercalate = false;
		var distance = 0;		
		var count = 0;
		const maxCount = 20;
		var realCount = 0;
		var exitResetFor = false;
		
		resultLine.push({x: x1, y: y1});
		
		RESET_FOR:while(!exitResetFor){
			realCount++;
			if (realCount >= maxCount){
				console.log("BREAK FORZADO");
				break RESET_FOR;
			}
			
			for (var count = 1; count < splLine.length; count++){
				
				distance = ShapeService.GetDistanceLine(splLine[count-1].x, splLine[count-1].y, splLine[count].x, splLine[count].y) / 2;
				var linesComparation = [
					{ x1: splLine[count-1].x, y1: splLine[count-1].y, x2: splLine[count].x, y2: splLine[count].y},
					{ x1: splLine[count-1].x, y1: splLine[count-1].y, x2: splLine[count-1].x + distance, y2: splLine[count-1].y}
				];
				
				if(!ShapeService.IsRightAngleBetweenLines(linesComparation)){					
					lines = ShapeService.SplitLine(splLine[count-1].x, splLine[count-1].y, splLine[count].x, splLine[count].y);
					
					var lineRadianAngle = ShapeService.GetRadianAngle(lines[0].x, lines[0].y, lines[1].x, lines[1].y);
					// distance = ShapeService.GetDistanceLine(lines[0].x, lines[0].y, lines[1].x, lines[1].y);
										
					var width = lines[2].x - lines[0].x;
					var height = lines[2].y - lines[0].y;
					const margin = 10;
					
					ctx.beginPath();
					ctx.lineWidth = 2.5;
					ctx.lineJoin = "round";
					
					resultLine.push({x: x1 + (margin * bd.x), y: y1 + (margin * bd.y)});
					
					// ctx.moveTo(lines[0].x, lines[0].y);
					
					width = Math.abs(width);
					height = Math.abs(height);
					
					if (width > height){
						
						resultLine.push({x: x1 + (margin * bd.x), y: y1 + (margin * bd.y)});
						
						ctx.lineTo(lines[1].x, lines[0].y);
						ctx.lineTo(lines[1].x, lines[2].y);
					}else{
						ctx.lineTo(lines[0].x, lines[1].y);
						ctx.lineTo(lines[2].x, lines[1].y);
						
						// ctx.lineTo(lines[0].x + margin, lines[0].y);
						// ctx.lineTo(lines[0].x + margin, lines[1].y);
						// ctx.lineTo(lines[2].x + margin, lines[1].y);
						// ctx.lineTo(lines[2].x + margin, lines[2].y);
						// ctx.lineTo(lines[2].x, lines[2].y);
					}
					ctx.lineTo(lines[2].x, lines[2].y);					
					ctx.stroke();
					ctx.lineJoin = "bevel";
					
					resultLine.push({x: lines[1].x, y: lines[1].y});
					splLine = [];
					splLine = [{x: lines[1].x, y: lines[1].y}, {x: lines[2].x, y: lines[2].y}];
					intercalate = !intercalate;
					return resultLine;
					
					continue RESET_FOR;
				}else{
					resultLine.push({x: splLine[count-1].x, y: splLine[count-1].y});
				}				
				
				if (count == splLine.length - 1){
					resultLine.push({x: splLine[count].x, y: splLine[count].y});
					break RESET_FOR;
				}
			}
			console.log("salir");
			exitResetFor = true;
		}
	}else{
		// resultLine.push({x: x1, y: y1}, {x: x2, y: y2});
	}
	
	return resultLine;
}

ShapeService.DrawTriangle = function(ctx, points){
	ctx.beginPath();
	
	for(let countPoint = 0; countPoint < points.length; countPoint++){		
		if (countPoint == 0){
			ctx.moveTo(points[countPoint].x, points[countPoint].y);
		}
		else{
			ctx.lineTo(points[countPoint].x, points[countPoint].y);
		}
		
		if (countPoint == 2){
			break;
		}
	}
	
	ctx.closePath();
	ctx.stroke();
    ctx.fill();	
}

ShapeService.DrawArrow = function(ctx, fromx, fromy, tox, toy, rotation){	
	var r = rotation;
	var x_center = tox;
	var y_center = toy;
	
	var angle;
	var x;
	var y;
	
	ctx.beginPath();	
	angle = Math.atan2(toy-fromy,tox-fromx)
	x = r*Math.cos(angle) + x_center;
	y = r*Math.sin(angle) + y_center;		
	ctx.moveTo(x, y);
	
	angle += (1/3)*(2*Math.PI)
	x = r*Math.cos(angle) + x_center;
	y = r*Math.sin(angle) + y_center;		
	ctx.lineTo(x, y);
	
	angle += (1/3)*(2*Math.PI)
	x = r*Math.cos(angle) + x_center;
	y = r*Math.sin(angle) + y_center;		
	ctx.lineTo(x, y);
	
	ctx.closePath();		
	ctx.fill();
}

function PolygonSelector(vertices, type)
{
	this.vertices = vertices || [];
	this.type = type || ShapeTypeEnum[ShapeTypeEnum.POLYGON];
	this.selectionColor = 'black';
	this.selectionWidth = 1.5;
}

PolygonSelector.prototype.SetVertices = function(vertices) {
	this.vertices = vertices || [];
}

PolygonSelector.prototype.Draw = function(ctx){
	
	var vertices = this.vertices;
	if (vertices.length > 0)
	{
		const margin = 0.5;		
		ctx.strokeStyle = this.selectionColor;
		ctx.lineWidth = this.selectionWidth;
		
		switch(this.type)
		{
			case ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]:
			case ShapeTypeEnum[ShapeTypeEnum.SQUARE]:			
			{
				ctx.beginPath();
				if (vertices[0].width && vertices[0].height){
					ctx.strokeRect(vertices[0].x - margin, vertices[0].y - margin, vertices[0].width + (margin * 2), vertices[0].height + (margin * 2));
				}else{
					var width = vertices[1].x - vertices[0].x;
					var height = vertices[1].y - vertices[0].y;
					ctx.strokeRect(vertices[0].x - margin, vertices[0].y - margin, width + (margin * 2), height + (margin * 2));
				}
			}
			break;
			case ShapeTypeEnum[ShapeTypeEnum.POLYGON]:
			{
				
			}
			break;
			case ShapeTypeEnum[ShapeTypeEnum.TRIANGLE]:
			{
				
			}
			break;
			case ShapeTypeEnum[ShapeTypeEnum.CIRCLE]:
			{
				
			}
			break;
			case ShapeTypeEnum[ShapeTypeEnum.LINE]:
			{
				var lineRadianAngle = ShapeService.GetRadianAngle(vertices[0].x, vertices[0].y, vertices[1].x, vertices[1].y);

				var oldComposite = ctx.globalCompositeOperation;
				var oldLineWith = ctx.lineWidth;
				
				// {
					// ctx.strokeStyle = "white";
					// ctx.globalCompositeOperation = "destination-over";
					// ctx.beginPath();
					// ctx.lineWidth = vertices[0].width + 2;
					// ctx.moveTo(vertices[0].x, vertices[0].y);
					// ctx.lineTo(vertices[1].x, vertices[1].y);
					// ctx.stroke();
					
					// const headLength = 10;
					// ctx.beginPath();
					// ctx.moveTo(vertices[1].x - headLength * Math.cos(lineRadianAngle - Math.PI / 6), vertices[1].y - headLength * Math.sin(lineRadianAngle - Math.PI / 6));
					// ctx.lineTo(vertices[1].x, vertices[1].y);
					// ctx.lineTo(vertices[1].x - headLength * Math.cos(lineRadianAngle + Math.PI / 6), vertices[1].y - headLength * Math.sin(lineRadianAngle + Math.PI / 6));	
					// ctx.stroke();
				// }
				
				{
					ctx.strokeStyle = "lightskyblue";
					ctx.globalCompositeOperation = "destination-over";
					ctx.beginPath();
					ctx.lineWidth = vertices[0].width + 5;
					ctx.moveTo(vertices[0].x, vertices[0].y);
					ctx.lineTo(vertices[1].x, vertices[1].y);
					ctx.stroke();
					
					const headLength = 10;
					ctx.beginPath();
					ctx.moveTo(vertices[1].x - headLength * Math.cos(lineRadianAngle - Math.PI / 6), vertices[1].y - headLength * Math.sin(lineRadianAngle - Math.PI / 6));
					ctx.lineTo(vertices[1].x, vertices[1].y);
					ctx.lineTo(vertices[1].x - headLength * Math.cos(lineRadianAngle + Math.PI / 6), vertices[1].y - headLength * Math.sin(lineRadianAngle + Math.PI / 6));	
					ctx.stroke();
				}				
				
				ctx.strokeStyle = "black";
				ctx.lineWidth = oldLineWith;
				ctx.globalCompositeOperation = oldComposite;
			}
			break;
			default:{				
			}
		}
	}
}

function PolygonConnector(vertices, type, directionArray){
	this.vertices = vertices || [];
	this.type = type || 'POLYGON';
	this.connectorSelected = null;
	this.connectors = [];
	this.directionArray = directionArray || [];
	
	if (!this.directionArray || this.directionArray.length == 0) {		
		this.SetDirectionByType(this.type);
	}
}

PolygonConnector.radius = 5;

PolygonConnector.prototype.SetDirection = function(directionArray)
{
	if (directionArray && Array.isArray(directionArray) && directionArray.length > 0)
	{
		this.directionArray = directionArray;
	}
}

PolygonConnector.prototype.SetDirectionByType = function(type)
{
		var directionArray = [];
		switch(type)
		{			
			case ShapeTypeEnum[ShapeTypeEnum.CIRCLE]:
			case ShapeTypeEnum[ShapeTypeEnum.RHOMBUS]:
				{
					directionArray = [				
											{ direction: "N",  inputQuantity: -1, outputQuantity: -1 },
											{ direction: "E",  inputQuantity: -1, outputQuantity: -1 },
											{ direction: "S",  inputQuantity: -1, outputQuantity: -1 },
											{ direction: "W",  inputQuantity: -1, outputQuantity: -1 }
									 ];				
				}
				break;
			default:
				{
					directionArray = [
											{ direction: "NW", inputQuantity: -1, outputQuantity: -1 },
											{ direction: "NE", inputQuantity: -1, outputQuantity: -1 },
											{ direction: "SE", inputQuantity: -1, outputQuantity: -1 },
											{ direction: "SW", inputQuantity: -1, outputQuantity: -1 },
											{ direction: "N",  inputQuantity: -1, outputQuantity: -1 },
											{ direction: "E",  inputQuantity: -1, outputQuantity: -1 },
											{ direction: "S",  inputQuantity: -1, outputQuantity: -1 },
											{ direction: "W",  inputQuantity: -1, outputQuantity: -1 }
									 ];
				}
				break;
		}

		this.directionArray = directionArray;
}

PolygonConnector.prototype.SetVertices = function(vertices) {
	if (this.vertices.length != vertices.length) {
		this.connectors = [];
	}
	
	this.vertices = vertices || [];
	const margin = 0.5;
	var lineWidth =  (vertices[0].width ? vertices[0].width : vertices[1].x - vertices[0].x) + (margin * 2)
	var lineHeight = (vertices[0].height ? vertices[0].height : vertices[1].y - vertices[0].y) + (margin * 2)
	var xAngle = vertices[0].x - margin;
	var yAngle = vertices[0].y - margin;
	const circleRadioSize = PolygonConnector.radius;

	var circleVerticesArray = [
		{ x: xAngle, 					y: yAngle, 						isVertice : true,	direction: 'NW', inputQuantity: -1, outputQuantity: -1 },
		{ x: xAngle + lineWidth, 		y: yAngle, 						isVertice : true, 	direction: 'NE', inputQuantity: -1, outputQuantity: -1 },
		{ x: xAngle + lineWidth, 		y: yAngle + lineHeight, 		isVertice : true, 	direction: 'SE', inputQuantity: -1, outputQuantity: -1 },
		{ x: xAngle, 					y: yAngle + lineHeight, 		isVertice : true, 	direction: 'SW', inputQuantity: -1, outputQuantity: -1 },	  
		{ x: xAngle + (lineWidth / 2), 	y: yAngle, 						isVertice : false, 	direction: 'N' , inputQuantity: -1, outputQuantity: -1 },
		{ x: xAngle + lineWidth, 		y: yAngle + (lineHeight / 2), 	isVertice : false, 	direction: 'E' , inputQuantity: -1, outputQuantity: -1 },
		{ x: xAngle + (lineWidth / 2), 	y: yAngle + lineHeight, 		isVertice : false, 	direction: 'S' , inputQuantity: -1, outputQuantity: -1 },
		{ x: xAngle, 					y: yAngle + (lineHeight / 2), 	isVertice : false, 	direction: 'W' , inputQuantity: -1, outputQuantity: -1 }
	];	
	
	if (this.directionArray != null)
	{
		var pushVertices = [];
		var directionLength = this.directionArray.length;			
		for (var countDirection = directionLength-1; countDirection >= 0; countDirection--) {	
			
			var verticesLength = circleVerticesArray.length;		
			for (var countVertices = verticesLength-1; countVertices >= 0; countVertices--) {			
				if (this.directionArray[countDirection].direction == circleVerticesArray[countVertices].direction) {
					circleVerticesArray[countVertices].inputQuantity = this.directionArray[countDirection].inputQuantity; // || -1;					
					circleVerticesArray[countVertices].outputQuantity = this.directionArray[countDirection].outputQuantity; // || -1;
					pushVertices.push(circleVerticesArray[countVertices]);	
					circleVerticesArray.splice(countVertices, 1);					
					break;
				}				
			}
			
			if (this.directionArray.length == 0) {
				break;
			}
		}
		
		if (pushVertices.length > 0){
			circleVerticesArray = pushVertices;
			this.connectors = [];
		}
	}
	
	for(var countCircle = 0; countCircle < circleVerticesArray.length; countCircle++)
	{
		if (this.connectors[countCircle]){
			this.connectors[countCircle] = {
				x: circleVerticesArray[countCircle].x,
				y: circleVerticesArray[countCircle].y,
				radius: circleRadioSize,
				isVertice: circleVerticesArray[countCircle].isVertice,
				direction: circleVerticesArray[countCircle].direction,
				inputQuantity: circleVerticesArray[countCircle].inputQuantity,
				outputQuantity: circleVerticesArray[countCircle].outputQuantity
			};
		}else{
			this.connectors.push({
				x: circleVerticesArray[countCircle].x,
				y: circleVerticesArray[countCircle].y,
				radius: circleRadioSize,
				isVertice: circleVerticesArray[countCircle].isVertice,
				direction: circleVerticesArray[countCircle].direction,
				inputQuantity: circleVerticesArray[countCircle].inputQuantity,
				outputQuantity: circleVerticesArray[countCircle].outputQuantity
			});
		}
	}
}

PolygonConnector.prototype.Copy = function (){
	var polygonConnector = new PolygonConnector(this.vertices, this.type, this.directionArray);
	return polygonConnector;
}

PolygonConnector.prototype.Draw = function (ctx){
	// DRAW CIRCLE CONNECTION	
	var connectors = this.connectors;
	if (connectors && connectors.length > 0)
	{
		for(var countCircle = 0; countCircle < connectors.length; countCircle++)
		{
			ctx.beginPath();
			ctx.lineWidth = 1;
			ctx.arc(connectors[countCircle].x, connectors[countCircle].y, connectors[countCircle].radius, 0, 2 * Math.PI);
			ctx.strokeStyle = "black";
			ctx.fillStyle = "lime";
			ctx.fill();
			ctx.stroke();
		}
	}
}

PolygonConnector.prototype.Contains = function(mx, my, connectorType) {
	var length = this.connectors.length;
	var inside = false;
	
	for(var i = 0; i < length; i++){
		if ((inside = ShapeService.DistanceCircle(mx, my, this.connectors[i].x, this.connectors[i].y) < this.connectors[i].radius))
		{
			this.connectorSelected = this.connectors[i];
			break;
		}
	}
	return inside;
}

function Line(beginX, beginY, endX, endY, fill, onlyOutputDirection)
{
	this.beginX = beginX || 0;
	this.beginY = beginY || 0;
	this.endX = endX || 0;
	this.endY = endY ||0;
	this.fill = fill || 'black';
	this.type = ShapeTypeEnum[ShapeTypeEnum.LINE];
	this.lineWidth = 2.4;
	this.lineRect = null;
	this.id = ShapeService.GetUniqueID();
	this.polygonSelector = null;
	this.directionMove = 1;
	this.externalProperties = {};
	this.dblClickFunction = null;
	this.onlyOutputDirection = onlyOutputDirection || false;
}

Line.Create = function(properties){
	var line = null;
	
	if (properties){
		line = new Line(properties.beginX, properties.beginY, properties.endX, properties.endY, properties.fill);
	}
	else{
		line = new Line(10, 10, 50, 10, 'lime');
	}
	
	return line;
}

Line.prototype.GetProperties = function(){
	var properties = {
		id: this.id,
		type: ShapeTypeEnum[ShapeTypeEnum.LINE],
		shapeProperties: { beginX: this.beginX, beginY: this.beginY, endX: this.endX, endY: this.endY, lineWidth: this.lineWidth, fill: this.fill },
		externalProperties: this.externalProperties
		};

	return properties;
}

// ESTO HACE QUE EL MOVIMIENTO DE LA LINEA COMIENCE DEL INICIO O FIN	
Line.prototype.GetDirectionToMove = function(bmx, bmy){
	var direction = 0;
	
	let distance = ShapeService.GetDistanceLine(this.beginX, this.beginY, bmx, bmy);
	const distanceBegin = 12;
	
	if (distance > distanceBegin){
		this.directionMove = direction = 1;
	}
	else{
		this.directionMove = direction = 0;
	}
	
	return direction;
}

Line.prototype.SetLine = function(beginX, beginY, endX, endY, fill){
	this.beginX = beginX || this.beginX || 0;
	this.beginY = beginY || this.beginY || 0;
	this.endX = endX || this.endX || 0;
	this.endY = endY || this.endY || 0;
	this.fill = fill || '#AAAAAA';
}

Line.prototype.Copy = function(){
	var line = new Line(this.beginX, this.beginY, this.endX, this.endY, this.fill, this.onlyOutputDirection); //this.type, this.lineWidth, this.lineRect);
	return line;
}

Line.prototype.Clone = function(){
	var line = this.Copy();
	line.id = this.id;	
	return line;
}

Line.prototype.Draw = function (ctx){
	// ctx.fillStyle = this.fill;
	ctx.lineWidth = this.lineWidth;
	ctx.strokeStyle = this.fill;
	
	// DRAW CIRCLE
	ctx.beginPath();
	ctx.arc(this.beginX, this.beginY, 1.2, 0, 2 * Math.PI);
	ctx.stroke();
	
	ctx.beginPath();
	ctx.moveTo(this.beginX, this.beginY);
	ctx.lineTo(this.endX, this.endY);
	ctx.stroke();
	
	//DRAW ARROW
	const headLength = 10;
	const angle = ShapeService.GetRadianAngle(this.beginX, this.beginY, this.endX, this.endY);
	ctx.beginPath();
	ctx.moveTo(this.endX - headLength * Math.cos(angle - Math.PI / 6), this.endY - headLength * Math.sin(angle - Math.PI / 6));
	ctx.lineTo(this.endX, this.endY);
	ctx.lineTo(this.endX - headLength * Math.cos(angle + Math.PI / 6), this.endY - headLength * Math.sin(angle + Math.PI / 6));	
	ctx.stroke();
	
	var vertices = [
		{ x : this.beginX, y : this.beginY, width: this.lineWidth },
		{ x : this.endX, y : this.endY, width: this.lineWidth }
	];	
	
	if (this.polygonSelector){
		this.polygonSelector.SetVertices(vertices);
		this.polygonSelector.Draw(ctx);
	}
}

Line.prototype.DrawRect = function (ctx) {
	this.lineRect = this.lineToRect(this.beginX, this.beginY, this.endX, this.endY, this.lineWidth);
	if (this.lineRect)
	{
		var rect = this.lineRect;
		ctx.beginPath();
		ctx.translate(rect.translateX, rect.translateY);
		ctx.rotate(rect.rotation);
		ctx.rect(rect.rectX, rect.rectY, rect.rectWidth, rect.rectHeight);
		ctx.translate(-rect.translateX, -rect.translateY);
		ctx.rotate(-rect.rotation);
		ctx.fillStyle = this.fill;
		ctx.strokeStyle = this.fill;
		ctx.fill();
		ctx.stroke();
		ctx.restore();
	}
}

Line.prototype.LineToRect = function(x1, y1, x2, y2, lineWidth){
	var dx = x2-x1; // deltaX used in length and angle calculations 
	var dy = y2-y1; // deltaY used in length and angle calculations
	var lineLength = Math.sqrt(dx*dx+dy*dy);
	var lineRadianAngle = Math.atan2(dy,dx);
	
	var result = {
		"translateX":x1,
		"translateY":y1,
		"rotation":lineRadianAngle,
		"rectX":0,
		"rectY":-lineWidth/2,
		"rectWidth":lineLength,
		"rectHeight":lineWidth
		};
	
	return result;
}

Line.prototype.Contains = function(mx, my)
{	
	var vertices = ShapeService.GetLineVertices(this.beginX, this.beginY, this.endX, this.endY, this.lineWidth + 4);
	var touch = {
		x : mx,
		y : my
	}
			
	return ShapeService.PointInPolygon(4, vertices, touch);
}

function Triangle(points, lineWidth, fill){
	this.points = points;
	this.lineWidth = lineWidth || 1;
	this.fill = fill || 'yellow';
	this.type = ShapeTypeEnum[ShapeTypeEnum.TRIANGLE];
	this.x = points[0].x;
	this.y = points[0].y;
	this.id = ShapeService.GetUniqueID();	
	this.externalProperties = {};
	this.dblClickFunction = null;
}

Triangle.Create = function(properties){
	var triangle = null;
	
	if (properties){
		triangle = new Triangle(properties.points, properties.lineWidth, properties.fill);
	}
	else{
		//var points = [ {x: mx, y: my}, {x: mx + 30, y: my - 60}, {x: mx + 60, y: my} ];				
		//triangle = new Triangle(points, 1, 'lime');
	}
	
	return triangle;
}

Triangle.prototype.GetProperties = function(){
	var properties = {
		id: this.id,
		type: ShapeTypeEnum[ShapeTypeEnum.TRIANGLE],
		shapeProperties: { x: this.x, y: this.y, points: this.points, lineWidth: this.lineWidth, fill: this.fill },
		externalProperties: this.externalProperties
		};

	return properties;
}

Triangle.prototype.GetInitCorner = function(){
	
}

Triangle.prototype.GetCenter = function(){
	var points = this.points;
	
	var h = ShapeService.GetDistanceLine(points[0].x, points[0].y, points[1].x, points[1].y);
	var w = ShapeService.GetDistanceLine(points[0].x, points[0].y, points[2].x, points[2].y);
	var minX = Math.min(points[0].x, points[1].x, points[2].x);
	var minY = Math.min(points[0].y, points[1].y, points[2].y);
	
	return ShapeService.GetCenterRectangle(minX, minY, w, h);
}

Triangle.prototype.ToCenter = function(){
	
}

Triangle.prototype.ToGrid = function(){
		
}

Triangle.prototype.SetPosition = function(x, y){
	// this.x = this.points[0].x = x;
	// this.y = this.points[0].y = y;
}

Triangle.prototype.Copy = function(){
	var triangle = new Triangle(this.points, this.lineWidth, this.fill);
	return triangle;
}

Triangle.prototype.Clone = function(){
	var triangle = this.Copy();
	triangle.id = this.id;	
	return triangle;
}

Triangle.prototype.Draw = function(ctx){
	ctx.beginPath();
	
	for(let countPoint = 0; countPoint < this.points.length; countPoint++){		
		if (countPoint == 0){
			ctx.moveTo(this.points[countPoint].x, this.points[countPoint].y);
		}
		else{
			ctx.lineTo(this.points[countPoint].x, this.points[countPoint].y);
		}
		
		if (countPoint == 2){
			break;
		}
	}
	
	ctx.closePath();
	ctx.stroke();
    ctx.fill();	
}

Triangle.prototype.Contains = function(mx, my){
	return false;
}

Triangle.prototype.ContainsWithMargin = function(mx, my, margin) {
	return false;
}

function Circle(x, y, radius, lineWidth, fill){
	this.x = x || 0;
	this.y = y || 0;
	this.h = radius;
	this.w = radius;
	this.radius = radius || 0;
	this.lineWidth = lineWidth || 1;
	this.fill = fill || 'lime';
	this.type = ShapeTypeEnum[ShapeTypeEnum.CIRCLE];
	this.id = ShapeService.GetUniqueID();
	this.polygonSelector = null;
	this.polygonConnector = new PolygonConnector(null, this.type);
	this.mouseOverShape = false;
	this.externalProperties = {};
	this.dblClickFunction = null;	
}

Circle.Create = function(properties){
	var circle = null;	
	if (properties){
		circle = new Circle(properties.x , properties.y, properties.radius, properties.lineWidth, properties.fill);
	}
	else{
		circle = new Circle(10, 10, 30, 1, 'lime');
	}
	
	return circle;
}

Circle.prototype.GetProperties = function(){
	var properties = {
		id: this.id,
		type: ShapeTypeEnum[ShapeTypeEnum.CIRCLE],
		shapeProperties: { x: this.x, y: this.y, radius: this.radius, fill: this.fill },
		externalProperties: this.externalProperties
		};

	return properties;
}

Circle.prototype.GetInitCorner = function(){
	
}

Circle.prototype.GetCenter = function(){
	var center = {x: this.x, y: this.y};	
	return center;
}

Circle.prototype.ToCenter = function(){
	
}

Circle.prototype.ToGrid = function(){
	var h = this.radius * 2;
	var w = this.radius * 2;
	
	this.x += this.radius;
	this.y += this.radius;
}

Circle.prototype.SetPosition = function(x, y){
	this.x = x;
	this.y = y;
}

Circle.prototype.SetProperties = function(properties){
	if (properties){
		this.x = properties.x || this.x || 0;
		this.y = properties.y || this.y || 0;
		this.radius = properties.radius || this.radius || 0;
		this.lineWidth = properties.lineWidth || this.lineWidth || 1;
		this.fill = properties.fill || this.fill || 'lime';
	}
}

Circle.prototype.Copy = function(){
	var circle = new Circle(this.x, this.y, this.radius, this.lineWidth, this.fill);
	return circle;
}

Circle.prototype.Clone = function(){
	var circle = this.Copy();
	circle.id = this.id;	
	return circle;
}

Circle.prototype.Draw = function(ctx){
	var center = { x: this.x + this.radius, y: this.y + this.radius };
	
	ctx.beginPath();
	ctx.lineWidth = 1;
	ctx.arc(this.x + this.radius, this.y + this.radius, this.radius, 0, 2*Math.PI);
	ctx.strokeStyle = "black";
	ctx.fillStyle = "lime";
	ctx.fill();
	ctx.stroke();
	
	var vertices = [{
			x : this.x,
			y : this.y,
			width : this.radius * 2,
			height : this.radius * 2
		}];
	
	if (this.polygonSelector){
		// this.polygonSelector.SetVertices(vertices);
		// this.polygonSelector.Draw(ctx);

		this.polygonConnector.SetVertices(vertices);
		this.polygonConnector.Draw(ctx);
	}
  
	if (this.mouseOverShape){
		this.polygonConnector.SetVertices(vertices);
		this.polygonConnector.Draw(ctx);
	}
}

Circle.prototype.Contains = function(mx, my){
	return (ShapeService.DistanceCircle(mx, my, this.x + this.radius, this.y + this.radius) < this.radius);
}

Circle.prototype.ContainsWithMargin = function(mx, my, margin) {
  return  (ShapeService.DistanceCircle(mx, my, this.x + this.radius, this.y + this.radius) < this.radius + margin)
}

Circle.prototype.GetLinePositionOnShape = function(beginX, beginY, endX, endY){	
	var position = {x: endX , y: endY};
	return position;
}

// Constructor for Shape objects to hold data for all drawn objects.
// For now they will just be defined as rectangles.
function Rectangle(x, y, w, h, fill) {
  // This is a very simple and unsafe constructor. All we're doing is checking if the values exist.
  // "x || 0" just means "if there is a value for x, use that. Otherwise use 0."
  // But we aren't checking anything else! We could put "Lalala" for the value of x 
  this.x = x || 0;
  this.y = y || 0;
  this.w = w || 0;
  this.h = h || 0;
  this.fill = fill || '#AAAAAA';
  this.type = ShapeTypeEnum[ShapeTypeEnum.RECTANGLE];
  this.polygonSelector = null;
  this.polygonConnector = new PolygonConnector(null, this.type);
  this.mouseOverShape = false;
  this.id = ShapeService.GetUniqueID();
  this.externalProperties = {};
  this.dblClickFunction = null;
  // LUIS VERA: SE PUEDE ENVIAR UN PARAMETRO LLAMADO externalProperties o metodo para poder guardar propiedades de la tarea
  // revisar como meter el if, puede ser por la linea una azul y otra amarilla
}

Rectangle.Create = function(properties){
	var rectangle = null;
	
	if (properties){
		rectangle = new Rectangle (properties.x, properties.y, properties.w, properties.h, properties.fill);
	}
	else{
		rectangle = new Rectangle (10, 10, 60, 60, 'lightskyblue');
	}
	
	return rectangle;	
}

Rectangle.prototype.Copy = function(){
	var shape = new Rectangle(this.x, this.y, this.w, this.h, this.fill);
	var vertices = [{
			x : this.x,
			y : this.y,
			width : this.w,
			height : this.h,
		}];
		
	return shape;
}

Rectangle.prototype.Clone = function(){
	var shape = this.Copy();
	shape.id = this.id;
	return shape;
}

Rectangle.prototype.GetProperties = function(){
	var properties = {
		id: this.id,
		type: ShapeTypeEnum[ShapeTypeEnum.RECTANGLE],
		shapeProperties: { x: this.x, y: this.y, w: this.w, h: this.h, fill: this.fill },
		externalProperties: this.externalProperties
		};

	return properties;
}

Rectangle.prototype.GetInitCorner = function(){
	
}

Rectangle.prototype.GetCenter = function(){	
	return ShapeService.GetCenterRectangle(this.x, this.y, this.w, this.h);
}

Rectangle.prototype.ToCenter = function(){
	
}

Rectangle.prototype.ToGrid = function(){
	
}

Rectangle.prototype.SetPosition = function(x, y){
	this.x = x;
	this.y = y;
}

Rectangle.prototype.SetRectangle = function(x, y, w, h){
	this.x = x || 0;
	this.y = y || 0;
	this.w = w || 0;
	this.h = h || 0;
}

// Draws this shape to a given context
Rectangle.prototype.Draw = function(ctx) {	
  ctx.fillStyle = this.fill;
  ctx.fillRect(this.x, this.y, this.w, this.h);  
  
  var vertices = [{
			x : this.x,
			y : this.y,
			width : this.w,
			height : this.h,
		}];
  
  if (this.polygonSelector){
	  this.polygonSelector.SetVertices(vertices);
	  this.polygonSelector.Draw(ctx);
	  
	  this.polygonConnector.SetVertices(vertices);
	  this.polygonConnector.Draw(ctx);
  }
  
  if (this.mouseOverShape){
	  this.polygonConnector.SetVertices(vertices);
	  this.polygonConnector.Draw(ctx);
  }
}

// Determine if a point is inside the shape's bounds
Rectangle.prototype.Contains = function(mx, my) {	
	// All we have to do is make sure the Mouse X,Y fall in the area between
	// the shape's X and (X + Width) and its Y and (Y + Height)
	return  (this.x <= mx) && (this.x + this.w >= mx) &&
          (this.y <= my) && (this.y + this.h >= my);
}

Rectangle.prototype.ContainsWithMargin = function(mx, my, margin) {
	return  (this.x - margin <= mx) && (this.x - margin + this.w + (margin * 2) >= mx) &&
          (this.y - margin <= my) && (this.y - margin + this.h + (margin * 2) >= my);
}

Rectangle.prototype.GetLinePositionOnShape = function(beginX, beginY, endX, endY){	
	var angle = ShapeService.GetRadianAngle(beginX, beginY, endX, endY);
	var lineDirection = this.GetLineDirection(endX, endY);
	var position = {x: endX , y: endY};
	
	switch(lineDirection)
	{
		case "N":
			position = {x: endX, y: this.y};
			break;
		case "S":
			position = {x: endX, y: this.y + this.h};
			break;
		case "E":
			position = {x: this.x + this.w, y: endY};
			break;
		case "W":
			position = {x: this.x, y: endY};
			break;
	}
		
	return position;
}

Rectangle.prototype.GetLineDirection = function(mx, my) {
	var center = ShapeService.GetCenterRectangle(this.x, this.y, this.w, this.h);
	var angle = ShapeService.GetRadianAngle(center.x, center.y, mx, my) * -1;
	var radianPI = angle*180/Math.PI;
	radianPI = (radianPI<0) ? (radianPI+360) : radianPI;
	radianPI = radianPI*12/360;
	var direction = null;
	
	const init = 1.4;
	const initN = init;
	const initW = initN+3;
	const initS = initW+3;
	const initE = initS+3;
	
	if (radianPI > initN && radianPI <= initW){
		direction = "N";
	}
	else if (radianPI > initW && radianPI <=initS){
		direction = "W";
	}
	else if (radianPI > initS && radianPI <=initE){
		direction = "S";
	}	
	else{
		direction = "E";
	}
	
	return direction;
}

function ToggleButton(id, text, x, y, w, h, textColor, backgroundColor, imageSource) {
	this.id = id || "";
	this.text = text || "";
	this.x = x || 0;
	this.y = y || 0;
	this.w = w || 10;
	this.h = h || 10;
	this.textColor = textColor || 'black';
	this.backgroundColor = backgroundColor || 'gray';
	this.imageSource = imageSource || null;
	this.toggle = false;
	this.type = "TOGGLE_BUTTON";
	this.imgButton = null;
	
	this.SetImage(imageSource);
}

ToggleButton.Create = function(id, text, x, y, w, h, textColor, backgroundColor, imageSource) {
	var toggleButton = new ToggleButton(id, text, x, y, w, h, textColor, backgroundColor, imageSource);
	return toggleButton;
}

ToggleButton.prototype.SetPosition = function(x, y){
	this.x = x;
	this.y = y;
}

ToggleButton.prototype.SetSize = function(w, h){
	this.w = w;
	this.h = h;
}

ToggleButton.prototype.LoadImage = function(imageSource){
	var imgButton = null;
	if (imageSource){
		// ShapeService.FileToBase64(imageSource);
		
		// imgButton = new Image();
		// imgButton.src = imageSource;
		// imgButton.crossOrigin = "anonymous";
		// // imgButton.setAttribute('crossorigin', 'anonymous');
		// var toggleButtonClass = this;
		
		// imgButton.onload = function(){
			// toggleButtonClass.isImageLoaded = true;
		// }
	}
	return imgButton;
}

ToggleButton.prototype.SetImage = function(imageSource){
	this.imgButton = null;
	if (imageSource){
		this.imgButton = this.LoadImage(imageSource);
	}
}

ToggleButton.prototype.Draw = function(ctx){
	if (ctx)
	{
		ctx.beginPath();
		ctx.fillStyle = this.backgroundColor;
		ctx.fillRect(this.x, this.y, this.w, this.h);
		
		if (this.imgButton){
			if (this.isImageLoaded){
				ctx.drawImage(this.imgButton, this.x, this.y, this.w, this.h);
			}
		}
		
		if (this.text && this.text.trim() != ""){			
			ctx.font = "30px Arial";
			ctx.fillStyle = "black";
			ctx.textAlign = "center";
			ctx.textBaseline = "middle" ;
			ctx.fillText(this.text, this.x + this.w / 2, this.y + this.h / 2);
		}
		
		if (this.toggle){
			ctx.beginPath();
			ctx.fillStyle = 'rgba(0,0,0,0.7)';
			ctx.rect(this.x - 2, this.y - 2, this.w + 4, this.h + 4);
			ctx.globalAlpha = 0.4;
			ctx.fill();
		}
		
		ctx.globalAlpha = 1;
	}
}

ToggleButton.prototype.Contains = function(mx, my){
	return  (this.x <= mx) && (this.x + this.w >= mx) &&
          (this.y <= my) && (this.y + this.h >= my);	
}

ToggleButton.prototype.SetToggleByCoordinates = function(mx, my){
	var contains = false;
	if (this.Contains(mx, my)){
		this.toggle = !this.toggle;
		contains = true;
	}
	return contains;
}

ToggleButton.prototype.SetToggle = function(toggle){
	this.toggle = toggle || false;
}

ToggleButton.prototype.IsToggle = function(){
	return this.toggle || false;
}

function Toolbar(x, y, cw, ch, space){
	this.controls = [];
	this.defaultControl = null;
	this.x = x || 0;
	this.y = y || 0;
	this.cw = cw || 40;
	this.ch = ch || 40;
	this.space = space || 5;
	this.controlSelected = null;
}

Toolbar.Create = function(x, y, cw, ch, space){	
	return new Toolbar(x, y, cw, ch, space);
}

Toolbar.prototype.Add = function(control, isDefault){
	isDefault = isDefault || false;
	let length = this.controls.length;
	let position = this.x + ((this.cw + this.space) * length) ;
	
	control.SetSize(this.cw, this.ch);
	control.SetPosition(position, this.y);	
	
	this.controls.push(control);
	
	if (isDefault){
		this.defaultControl = control;
	}
}

Toolbar.prototype.ClearSelection = function(){
	this.controlSelected = this.defaultControl;
	
	for (let countBar = 0; countBar < this.controls.length; countBar++){
		this.controls[countBar].SetToggle(false);
	}
	
	if (this.controlSelected){
		this.controlSelected.SetToggle(true);
	}
}

Toolbar.prototype.GetControlSelected = function(){
	return this.controlSelected;
}

Toolbar.prototype.GetIdSelected = function(){
	var id = null;
	if (this.controlSelected){
		id = this.controlSelected.id;
	}	
	return id;
}

Toolbar.prototype.Toggle = function(mx, my){
	if (this.controls.length > 0){
		var clickInToolbar = false;
		var toolbarSelected = false;
		for (let countBar = 0; countBar < this.controls.length; countBar++){
			if (!toolbarSelected && (clickInToolbar = this.controls[countBar].SetToggleByCoordinates(mx, my))){
				toolbarSelected = true;
				
				if (!this.controls[countBar].IsToggle()){					
					this.defaultControl.SetToggle(true);
					this.controlSelected = this.defaultControl;
				}else{
					this.controlSelected = this.controls[countBar];
				}
				
			}
		}
		
		if (this.controlSelected){			
			for (let countBar = 0; countBar < this.controls.length; countBar++){
				if (this.controlSelected.id != this.controls[countBar].id){
					this.controls[countBar].SetToggle(false);
				}
			}
		}		
	}
}

Toolbar.prototype.Draw = function(ctx){
	for (let countBar = 0; countBar < this.controls.length; countBar++){		
		this.controls[countBar].Draw(ctx);
	}
}

function CanvasState(canvas, freeStyle, freeLines, onlyOutputLine, gridSize) {
  // **** First some setup! **** 
  this.uniqueID = 0;
  this.canvas = canvas;
  this.width = canvas.width;
  this.height = canvas.height;  
  this.ctx = canvas.getContext('2d');
  // This complicates things a little but but fixes mouse co-ordinate problems
  // when there's a border or padding. See getMouse for more detail
  var stylePaddingLeft, stylePaddingTop, styleBorderLeft, styleBorderTop;
  if (document.defaultView && document.defaultView.getComputedStyle) {
    this.stylePaddingLeft = parseInt(document.defaultView.getComputedStyle(canvas, null)['paddingLeft'], 10)      || 0;
    this.stylePaddingTop  = parseInt(document.defaultView.getComputedStyle(canvas, null)['paddingTop'], 10)       || 0;
    this.styleBorderLeft  = parseInt(document.defaultView.getComputedStyle(canvas, null)['borderLeftWidth'], 10)  || 0;
    this.styleBorderTop   = parseInt(document.defaultView.getComputedStyle(canvas, null)['borderTopWidth'], 10)   || 0;
  }
  // Some pages have fixed-position bars (like the stumbleupon bar) at the top or left of the page
  // They will mess up mouse coordinates and this fixes that
  var html = document.body.parentNode;
  this.htmlTop = html.offsetTop;
  this.htmlLeft = html.offsetLeft;

  // **** Keep track of state! ****  
  this.valid = false; // when set to false, the canvas will redraw everything
  this.shapes = [];  // the collection of things to be drawn
  this.lines = [];
  // this.polygonSelectors = [];
  this.dragging = false; // Keep track of when we are dragging
  // the current selected object. In the future we could turn this into an array for multiple selection
  this.selection = null;
  this.overSelection = null;
  this.beginSelection = null;
  this.beginShapeLines = [];
  this.dragoffx = 0; // See mousedown and mousemove events for explanation
  this.dragoffy = 0;
  this.beginMouseX = 0;
  this.beginMouseY = 0;
  this.movingMouseX = 0;
  this.movingMouseY = 0;
  this.drawSelector = false;
  this.screenShot = {};
  this.multipleSelection = [];
  this.copySelection = {};
  this.canvasResizeDraw = false;
  this.canvasResizeCoordinates = {x: 0, y: 0, w: 0, h: 0, drawX: false, drawY: false};
  this.freeStyle = freeStyle || false;
  this.freeLines = freeLines || false;
  this.urManager = new UndoRedoManager();
  this.shapeLines = [];
  this.shapeLinesMoved = false;
  this.isNewLine = false;
  this.shapeTypeSelected = ShapeTypeEnum[ShapeTypeEnum.RECTANGLE];
  this.gridSize = gridSize || 100;
  this.margin = 60;
  this.lineDirection = 1;
  this.onlyOutputLine = onlyOutputLine || false;
  this.toolbar = Toolbar.Create(5, 5, 40, 40, 5);  
  
  // this.shapesList = [];  
  // this.shapesList[ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]] = {Create: Rectangle.Create};
  // this.shapesList[ShapeTypeEnum[ShapeTypeEnum.CIRCLE]] = {Create: Circle.Create};
  // this.shapesList[ShapeTypeEnum[ShapeTypeEnum.LINE]] = {Create: Line.Create};
  
  this.toolbar.Add(ToggleButton.Create("DEFAULT", "", 10, 10, 40, 40, "black", "silver", "Shapes/Cursor.png"), true);
  this.toolbar.Add(ToggleButton.Create(ShapeTypeEnum[ShapeTypeEnum.LINE], "L", 55, 10, 40, 40, "black", "lightskyblue", "Shapes/Line.png"));
  this.toolbar.Add(ToggleButton.Create(ShapeTypeEnum[ShapeTypeEnum.RECTANGLE], "R", 100, 10, 40, 40, "black", "blue", "Shapes/Rectangle.jpg"));
  this.toolbar.Add(ToggleButton.Create(ShapeTypeEnum[ShapeTypeEnum.CIRCLE], "C", 145, 10, 40, 40, "black", "lime", "Shapes/Circle.jpg"));
  this.toolbar.Add(ToggleButton.Create(ShapeTypeEnum[ShapeTypeEnum.TRIANGLE], "T", 145, 10, 40, 40, "black", "yellow", "Shapes/Triangle.png"));
  
  // **** Then events! ****
  
  // This is an example of a closure!
  // Right here "this" means the CanvasState. But we are making events on the Canvas itself,
  // and when the events are fired on the canvas the variable "this" is going to mean the canvas!
  // Since we still want to use this particular CanvasState in the events we have to save a reference to it.
  // This is our reference!
  var myState = this;
  
  //fixes a problem where double clicking causes text to get selected on the canvas
  canvas.addEventListener('selectstart', function(e) { e.preventDefault(); return false; }, false);
  
  // Up, down, and move are for dragging
  canvas.addEventListener('mousedown', function(e) {
    var mouse = myState.GetMouse(e);
    var mx = mouse.x;
    var my = mouse.y;
	
	myState.OnMouseDownBegin(mx, my);
	
	// UNSELECT LINE SHAPE
	if (myState.selection && myState.selection.polygonSelector && myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.LINE]){
		myState.selection.polygonSelector = null;
	}
		
	if (myState.OnResizeCanvas(mx, my)){
		return;
	}
	
	if (myState.toolbar){
		myState.toolbar.Toggle(mx, my);
	}
	
	if (myState.ContainsMultipleSelectionOnMouseDown(mx, my)){
		return;
	}
	
	if (myState.ContainsLineOnMouseDown(mx, my)){
		return;
	}else if (myState.ContainsNewLine(mx, my)){
		return;
	}
	
	if (myState.ContainsShapeOnMouseDown(mx, my)){
		return;
	}
	
	DrawSelector.Restart({x: mx, y: my });
	myState.drawSelector = true;
	
    // havent returned means we have failed to select anything.
    // If there was an object selected, we deselect it
    if (myState.selection) {
		myState.selection = null;
		myState.beginSelection = null;
		myState.valid = false; // Need to clear the old selection border
    }
	
  }, true);
  
    
  canvas.addEventListener('mousemove', function(e) {
	var mouse = myState.GetMouse(e);
	var mx = mouse.x;
    var my = mouse.y;
	myState.OnMouseMoveBegin(mx, my);
	
	myState.OnDraggingShape(mx, my);	
	myState.OnDraggingSelector(mx, my);
	myState.OnOverShape(mx, my);	
	myState.OnDrawResize(mx, my);
	
	myState.OnMouseMoveEnd(mx, my);
	
  }, true);
    
  canvas.addEventListener('mouseup', function(e) {
	var mouse = myState.GetMouse(e);
	var mx = mouse.x;
    var my = mouse.y;
	
	if (myState.selection) {
		if (myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.LINE]){
			// // LUIS VERA AQUI SE PUEDE PONER el lineToSegments
			// var inConnector = myState.getConnectorByCoordinate(myState.selection.beginX, myState.selection.beginY);
			// var outConnector = myState.getConnectorByCoordinate(myState.selection.endX, myState.selection.endY);
			
			// if (outConnector != null){
				// ShapeService.lineToSegments(myState.selection.beginX, myState.selection.beginY, 
					// myState.selection.endX, myState.selection.endY, 
					// ShapeService.directionToVector(inConnector.direction), ShapeService.directionToVector(outConnector.direction),
					// myState.ctx);
			// }
		}else{			
			myState.OnMouseUpShape(mx, my);
		}
	}
	
	myState.OnMouseUpScreenShot(mx, my);
	myState.OnMouseUpLine(mx, my);
	myState.OnMouseUpMultipleSelection(mx, my);	
	myState.OnMouseUpEnd(mx, my);
	
  }, true);
  
  canvas.addEventListener('mouseleave', function(e) {
	myState.dragging = false;
  });
    
  canvas.addEventListener('dblclick', function(e) {
    var mouse = myState.GetMouse(e);	
	var mx = myState.beginMouseX;
	var my = myState.beginMouseY;
	
	if (myState.overSelection != null)
	{
		alert("Doble click en id: " + myState.overSelection.id.toString());		
		if (myState.overSelection.dblClickFunction)
		{
			myState.overSelection.dblClickFunction(myState.overSelection.externalProperties);
		}
		
	}
	else
	{
		switch(myState.toolbar.GetIdSelected())
		{
			case ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]:			
				myState.InsertShape(Rectangle.Create({x: mx, y: my, w: 60, h: 60, fill: 'lightskyblue'}));
				break;
			case ShapeTypeEnum[ShapeTypeEnum.CIRCLE]:
				myState.InsertShape(Circle.Create({x: mx, y: my, radius: 30, lineWidth: 1, fill: 'lightskyblue'}));
				break;
			case ShapeTypeEnum[ShapeTypeEnum.TRIANGLE]:{
					var points = [ {x: mx, y: my}, {x: mx + 30, y: my - 60}, {x: mx + 60, y: my} ];				
					myState.InsertShape(Triangle.Create({points: points, lineWidth: 1, fill: 'lightskyblue'}));
				}
				break;
			case ShapeTypeEnum[ShapeTypeEnum.LINE]:
				myState.InsertShape(Line.Create({beginX: mx, beginY: my, endX: mx + 100, endY: my, fill: 'black'}));
				break;
		}
		
		myState.toolbar.ClearSelection();		
	}
  }, true);
  
  canvas.addEventListener('contextmenu', function(e){
	e.preventDefault();
	e.stopPropagation();
	var mouse = myState.GetMouse(e);
	
  }, false);
  
  // **** Options! ****  
  this.selectionColor = 'black';//'#CC0000';
  this.selectionWidth = 1;  
  
  this.interval = 30;  
  // setInterval(function() { myState.Draw(); }, myState.interval);
  setTimeout(function(){ myState.MainLoop(myState); }, 30);  
}

CanvasState.prototype.OnMouseUpEnd = function(mx, my){
	var myState = this;	
	myState.dragging = false;
}

CanvasState.prototype.OnMouseUpMultipleSelection = function(mx, my){
	var myState = this;
	
	// MULTIPLE SELECTION
	if (myState.drawSelector){
		myState.multipleSelection = [];
		var minX = 0;
		var minY = 0;
		var maxX = 0;
		var maxY = 0;
		var width = 0;
		var height = 0;
		var rect = DrawSelector.ToRect();		
		let shapes = myState.shapes;
		let l = shapes.length;
		for (var i = l-1; i >= 0; i--) {
			const margin = 5;
			var mySel = shapes[i];
			if (DrawSelector.ContainsRect(shapes[i].x, shapes[i].y, shapes[i].w, shapes[i].h)) {				
				shapes[i].polygonSelector = new PolygonSelector([{
					x : shapes[i].x,
					y : shapes[i].y,
					width : shapes[i].w,
					height : shapes[i].h
				}]);
				
				if (minX == 0){ minX = shapes[i].x; }
				if (minY == 0){ minY = shapes[i].y; }
				if (maxX == 0){ maxX = shapes[i].x + shapes[i].w; }
				if (maxY == 0){ maxY = shapes[i].y + shapes[i].h; }
				
				if (minX > shapes[i].x){ minX = shapes[i].x; }
				if (minY > shapes[i].y){ minY = shapes[i].y; }
				if (maxX < shapes[i].x + shapes[i].w){ maxX = shapes[i].x + shapes[i].w; }
				if (maxY < shapes[i].y + shapes[i].h){ maxY = shapes[i].y + shapes[i].h; }
				
				myState.multipleSelection.push({shape: shapes[i], type: 'shape', full: true});
			}
		}
		
		let lines = myState.lines;
		let maxLines = lines.length;
		for (var i = maxLines-1; i >= 0; i--) {
			const margin = 5;
			var mySel = lines[i];			
			var x = lines[i].beginX > lines[i].endX ? lines[i].endX : lines[i].beginX;
			var y = lines[i].beginY > lines[i].endY ? lines[i].endY : lines[i].beginY;
			var width = Math.abs(lines[i].beginX - lines[i].endX);
			var height = Math.abs(lines[i].beginY - lines[i].endY);
			if (DrawSelector.ContainsRect(x, y, width, height)) {
				
				if (minX == 0){ minX = x; }
				if (minY == 0){ minY = y; }
				if (maxX == 0){ maxX = x + width; }
				if (maxY == 0){ maxY = y + height; }
				
				if (minX > x){ minX = x; }
				if (minY > y){ minY = y; }
				if (maxX < x + width){ maxX = x + width; }
				if (maxY < y + height){ maxY = y + height; }
				
				myState.multipleSelection.push({shape: lines[i], type: ShapeTypeEnum[ShapeTypeEnum.LINE], full: true});
			}else if (DrawSelector.ContainsPoint(lines[i].beginX, lines[i].beginY)){				
				myState.multipleSelection.push({shape: lines[i], type: ShapeTypeEnum[ShapeTypeEnum.LINE], full: false, move: 'begin'});
			}else if (DrawSelector.ContainsPoint(lines[i].endX, lines[i].endY)){				
				myState.multipleSelection.push({shape: lines[i], type: ShapeTypeEnum[ShapeTypeEnum.LINE], full: false, move: 'end'});
			}
		}
		
		if (myState.multipleSelection.length > 0){
			setTimeout(function(){
				var canvasBase64 = myState.canvas.toDataURL("image/png");
				myState.CaptureScreen(canvasBase64, minX, minY, maxX - minX, maxY - minY, 0, 0, maxX - minX, maxY - minY, true);
				myState.valid = false;
			}, 20);
		}
		
		myState.drawSelector = false;
		myState.valid = false;
	}
}

CanvasState.prototype.OnMouseUpLine = function(mx, my){
	var myState = this;
	if (myState.selection && myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.LINE]){
		
		// DELETE LINE IF THE POINT IS NOT ON THE OVER SHAPE AND freeLines IS DISABLED
		let line = myState.selection;
		if (!myState.freeLines && (myState.overSelection == null || myState.selection.overSelection == ShapeTypeEnum[ShapeTypeEnum.LINE]))
		{
			if (myState.selection != null)
			{
				//  UNDOREDO CUANDO LA LINEA YA EXISTE Y LA INTENTAN REASIGNAR PERO CAE FUERA DE UN SHAPE				
				if (!myState.isNewLine)
				{	
					line.SetLine(myState.beginSelection.beginX, myState.beginSelection.beginY, 
								myState.beginSelection.endX, myState.beginSelection.endY, 'black');
								
					myState.DeleteShape(line);				
				}else
				{				
					myState.DeleteLine(myState.selection);
				}
			}
			myState.valid = false;
		} // DELETE LINE IF THE POINT IS THE SAME TO BEGIN
		else if (ShapeService.DistanceCircle(line.endX, line.endY, line.beginX, line.beginY) < PolygonConnector.radius)
		{
			myState.DeleteLine(line);
			myState.valid = false;
		}
		else
		{
			// ADD REFERENCE LINE AT THE INPUT CONNECTOR						
			if (myState.isNewLine){
				myState.DeleteLine(line);
				myState.InsertShape(line);
			}else{
				myState.urManager.AddObject(UndoRedoActionEnum.UPDATE, myState.beginSelection, line.Clone());
			}
			
			
			var shape = null;
			// var inputConnector = myState.ContainsConnector(line.endX, line.endY, 'IN_LINE');
			// if (inputConnector){
				// line.endX = inputConnector.x;
				// line.endY = inputConnector.y;
				// myState.valid = false;
			// }
			// else
			{
				if (myState.selection.directionMove == 0){	
					if ((shape = myState.ContainsLineOnShape(line.beginX, line.beginY))){						
						let position = shape.GetLinePositionOnShape(line.endX, line.endY, line.beginX, line.beginY);
						line.SetLine(position.x, position.y, null, null, 'black');						
					}
				}
				else{
					if ((shape = myState.ContainsLineOnShape(line.endX, line.endY))){
						let position = shape.GetLinePositionOnShape(line.beginX, line.beginY, line.endX, line.endY);
						line.SetLine(null, null, position.x, position.y, 'black');
					}
				}
			}
		}
	}
}

CanvasState.prototype.OnMouseUpScreenShot = function(mx, my){
	var myState = this;
	
	if (myState.screenShot && myState.screenShot.show){		
		myState.screenShot.show = false;	
		
		var l = myState.multipleSelection.length;
		var shapes = myState.multipleSelection;		
		var undoShapes = [];
		var inverseShapes = [];
		var minX = 0, minY = 0, maxX = 0, maxY = 0;
		
		for (var i = l-1; i >= 0; i--) {
			undoShapes.push(shapes[i].shape.Clone());
			
			if (shapes[i].type == 'shape'){
				shapes[i].shape.x = shapes[i].shape.x + myState.screenShot.dx - myState.screenShot.minX;
				shapes[i].shape.y = shapes[i].shape.y + myState.screenShot.dy - myState.screenShot.minY;			
				myState.PositionShapeInGrid(shapes[i].shape);			
			
				if (minX == 0){ minX = shapes[i].shape.x; }
				if (minY == 0){ minY = shapes[i].shape.y; }
				if (maxX == 0){ maxX = shapes[i].shape.x + shapes[i].shape.w; }
				if (maxY == 0){ maxY = shapes[i].shape.y + shapes[i].shape.h; }
				
				if (minX > shapes[i].shape.x){ minX = shapes[i].shape.x; }
				if (minY > shapes[i].shape.y){ minY = shapes[i].shape.y; }
				if (maxX < shapes[i].shape.x + shapes[i].shape.w){ maxX = shapes[i].shape.x + shapes[i].shape.w; }
				if (maxY < shapes[i].shape.y + shapes[i].shape.h){ maxY = shapes[i].shape.y + shapes[i].shape.h; }
			}
			else if (shapes[i].type == ShapeTypeEnum[ShapeTypeEnum.LINE]){
						
				if (shapes[i].full == true){
					shapes[i].shape.beginX += myState.screenShot.dx - myState.screenShot.minX;
					shapes[i].shape.beginY += myState.screenShot.dy - myState.screenShot.minY;
					shapes[i].shape.endX += myState.screenShot.dx - myState.screenShot.minX;
					shapes[i].shape.endY += myState.screenShot.dy - myState.screenShot.minY;
					
					myState.PositionLineInGrid(shapes[i].shape, myState.screenShot.dx, myState.screenShot.dy, true, true, false);
				}else if (shapes[i].move == 'begin'){
					shapes[i].shape.beginX += myState.screenShot.dx - myState.screenShot.minX;
					shapes[i].shape.beginY += myState.screenShot.dy - myState.screenShot.minY;					
					myState.PositionLineInGrid(shapes[i].shape, myState.screenShot.dx, myState.screenShot.dy, true, false, false);					
				}else{					
					shapes[i].shape.endX += myState.screenShot.dx - myState.screenShot.minX;
					shapes[i].shape.endY += myState.screenShot.dy - myState.screenShot.minY;
					myState.PositionLineInGrid(shapes[i].shape, myState.screenShot.dx, myState.screenShot.dy, false, true, false);
				}
			
				// if (minX == 0){ minX = shapes[i].shape.x; }
				// if (minY == 0){ minY = shapes[i].shape.y; }
				// if (maxX == 0){ maxX = shapes[i].shape.x + shapes[i].shape.w; }
				// if (maxY == 0){ maxY = shapes[i].shape.y + shapes[i].shape.h; }
				
				// if (minX > shapes[i].shape.x){ minX = shapes[i].shape.x; }
				// if (minY > shapes[i].shape.y){ minY = shapes[i].shape.y; }
				// if (maxX < shapes[i].shape.x + shapes[i].shape.w){ maxX = shapes[i].shape.x + shapes[i].shape.w; }
				// if (maxY < shapes[i].shape.y + shapes[i].shape.h){ maxY = shapes[i].shape.y + shapes[i].shape.h; }
			}
			
			inverseShapes.push(shapes[i].shape.Clone());
		}
		
		if (undoShapes.length > 0){
			myState.urManager.AddObject(UndoRedoActionEnum.UPDATE, undoShapes, inverseShapes);
		}

		setTimeout(function(){ 
			var canvasImage = myState.canvas.toDataURL("image/png");
			
			if (myState.screenShot.crop){
				myState.CropImage(function(result){
					myState.screenShot.image = result;
					}, canvasImage, minX, minY, 
					myState.screenShot.sWidth, myState.screenShot.sHeight, 0, 0, 
					myState.screenShot.dWidth, myState.screenShot.dHeight);
			}else{
				myState.screenShot.image = canvasImage;
			}			
			
			myState.screenShot.sx = myState.screenShot.crop ? 0 : minX;
			myState.screenShot.sy = myState.screenShot.crop ? 0 : minY;
			myState.screenShot.minX = minX;
			myState.screenShot.minY = minY;			
			// myState.screenShot.w = Math.abs(maxX - minX);
			// myState.screenShot.h = Math.abs(maxY - minY);
			myState.valid = false;
		}, 20);
		
		myState.valid = false;
	}
}

CanvasState.prototype.OnMouseUpShape = function(mx, my){
	var myState = this;
	if (myState.selection && myState.selection.type != ShapeTypeEnum[ShapeTypeEnum.LINE]
		&& !(myState.screenShot && myState.screenShot.show))
	{
		var shapesRedo = [];
		var shapesUndo = [];
		var beginX = myState.selection.x;
		var beginY = myState.selection.y;
		
		myState.PositionShapeInGrid(myState.selection);
		
		var shapeClone = myState.selection.Clone();
		shapesUndo.push(myState.beginSelection);
		shapesRedo.push(shapeClone);
		
		if (myState.shapeLines && myState.shapeLines.length > 0)
		{
			lines = myState.shapeLines;
			maxLines = myState.shapeLines.length;
			var linesCloned = [];
			for (var countLines = maxLines-1; countLines >= 0; countLines--) {
				var line = myState.shapeLines[countLines].line;
				if (myState.shapeLines[countLines].type == 0){
					myState.PositionLineInGrid(line, beginX, beginY, true, false, false);
				}
				else{
					myState.PositionLineInGrid(line, beginX, beginY, false, true, false);
				}
				shapesRedo.push(line.Clone());
			}
		}
		
		if (myState.beginShapeLines && myState.beginShapeLines.length > 0){
			for(let countLines = 0; countLines < myState.beginShapeLines.length; countLines++){
				shapesUndo.push(myState.beginShapeLines[countLines]);
			}
		}
		
		myState.urManager.AddObject(UndoRedoActionEnum.UPDATE, shapesUndo, shapesRedo);
	}
}


CanvasState.prototype.OnDrawResize = function(mx, my){
	var myState = this;
	// DRAW RECT CANVAS SIZE	
	if (!myState.drawSelector && !myState.dragging){
		myState.CanvasResize(mx, my);
	}
}

CanvasState.prototype.OnOverShape = function(mx, my){
	const margin = 5;
	// CHECK IF MOUSE IS OVER SQUARE SHAPE
	// if (myState.selection == null || (myState.selection && myState.selection.type != ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]))
	{		
		var myState = this
		var shapes = myState.shapes;
		var l = shapes.length;		
		myState.overSelection = null;
		
		for (var i = l-1; i >= 0; i--) {			
			var mySel = shapes[i];
			if (shapes[i].ContainsWithMargin(mx, my, margin)) {
				mySel.mouseOverShape = true;
				myState.overSelection = mySel;
			}else{
				mySel.mouseOverShape = false;
			}			
			myState.valid = false
		}
	}
}

CanvasState.prototype.OnDraggingSelector = function(mx, my){
	var myState = this;
	if (myState.drawSelector){
		DrawSelector.Update({x: mx, y: my});
		myState.CanvasSlide(mx, my);
	}
}

CanvasState.prototype.OnDraggingShape = function(mx, my){
	var myState = this;
	
	// We don't want to drag the object by its top-left corner, we want to drag it
    // from where we clicked. Thats why we saved the offset and use it here
    if (myState.dragging){
		myState.drawSelector = false;
		
		if (myState.multipleSelection.length > 0){		  
			myState.screenShot.dx =  myState.screenShot.minX + ((mx - myState.dragoffx) - myState.selection.x);
			myState.screenShot.dy =  myState.screenShot.minY + ((my - myState.dragoffy) - myState.selection.y);
			myState.screenShot.show = true;
		}
		else if (myState.selection){			
			if (myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]
				|| myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.CIRCLE]
				|| myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.TRIANGLE]){
				myState.MoveLineOnShape(myState.movingMouseX, myState.movingMouseY, mx, my);
				myState.selection.x = mx - myState.dragoffx;
				myState.selection.y = my - myState.dragoffy;
				
			} 
			else if (myState.selection.type == ShapeTypeEnum[ShapeTypeEnum.LINE]){
				if (myState.selection.directionMove == 0){
					myState.selection.SetLine(mx, my, null, null, 'black');
				}
				else{
					myState.selection.SetLine(null, null, mx, my, 'black');
				}
			}
		}
		
		myState.CanvasSlide(mx, my);
		myState.valid = false; // Something's dragging so we must redraw
    }
}

CanvasState.prototype.OnResizeCanvas = function(mx, my){
	var myState = this;
	var resizeCoordinates = myState.canvasResizeCoordinates;
	if ((resizeCoordinates.x <= mx) && (resizeCoordinates.x + resizeCoordinates.w >= mx) &&
          (resizeCoordinates.y <= my) && (resizeCoordinates.y + resizeCoordinates.h >= my))
	{		
		setTimeout(function(){
			const size = 300;
			if (resizeCoordinates.drawX){
				myState.canvas.width += size;
			}
			else if (resizeCoordinates.drawY){
				myState.canvas.height += size;
			}
			
			myState.width = myState.canvas.width;
			myState.height = myState.canvas.height;
			myState.valid = false;
		}, 20);
		
		return;		
	}
} 

CanvasState.prototype.ContainsMultipleSelectionOnMouseDown = function(mx, my){
	var myState = this;
	let length = 0;
	
	// MOVE MULTIPLE SELECTION
	var multipleSelection = myState.multipleSelection;
	if (multipleSelection && multipleSelection.length > 0){
		length = multipleSelection.length;		
		
		for (var i = length-1; i >= 0; i--) {		  
		  if (multipleSelection[i].type == 'shape' && multipleSelection[i].shape.Contains(mx, my)) {
			var mySel = multipleSelection[i].shape;
			myState.dragoffx = mx - mySel.x;
			myState.dragoffy = my - mySel.y;
			myState.dragging = true;			
			myState.valid = false;
			myState.selection = mySel;
			return true;
		  }
		}
		
		// REMOVE MULTIPLE SELECTION
		for (var i = length-1; i >= 0; i--) {
			if (multipleSelection[i].type == 'shape'){
				multipleSelection[i].shape.polygonSelector = null;
			}
		}
		
		myState.multipleSelection = [];
	}
	
	myState.screenShot = {};
	return false;
}

CanvasState.prototype.ContainsShapeOnMouseDown = function(mx, my){
	var myState = this;
	var shapes = myState.shapes;
	var objectSelected = false;
	let maxShapes = shapes.length;	
    for (var i = maxShapes-1; i >= 0; i--) {
	  // SE LIMPIA EL SELECTOR PARA SELECCIONAR ALGÚN OTRO
	  shapes[i].polygonSelector = null;
      if (shapes[i].Contains(mx, my)) {
        var mySel = shapes[i];
        // Keep track of where in the object we clicked
        // so we can move it smoothly (see mousemove)
        myState.dragoffx = mx - mySel.x;
        myState.dragoffy = my - mySel.y;
        myState.dragging = true;
        myState.selection = mySel;
		
		myState.selection.polygonSelector = new PolygonSelector([{
			x : mySel.x,
			y : mySel.y,
			width : mySel.w,
			height : mySel.h,
			radius : mySel.radius,
		}], shapes[i].type);
		
		myState.valid = false;		
		objectSelected = true;
		myState.beginSelection = mySel.Clone();		
		myState.GetLinesFromShape(mySel, mx, my);        
      }
    }
	
	return objectSelected;
	
}

CanvasState.prototype.OnMouseDownBegin = function(mx, my){
	var myState = this;
	
	myState.beginMouseX = mx;
	myState.beginMouseY = my;
	myState.beginMovingMouseX = mx;
	myState.beginMovingMouseY = my;
	myState.isNewLine = false;
	myState.shapeLinesMoved = false;
	myState.shapeLines = [];
	myState.beginShapeLines = [];
}

CanvasState.prototype.OnMouseMoveBegin = function(mx, my){
	var myState = this;
	myState.canvasResizeDraw = false;	
}

CanvasState.prototype.OnMouseMoveEnd = function(mx, my){
	var myState = this;
	myState.movingMouseX = mx;
	myState.movingMouseY = my;
}

CanvasState.prototype.ContainsNewLine = function(mx, my){
	var result = false;
	var myState = this;
	
	var connector = null;
	if ((connector = myState.ContainsConnector(mx, my, 'OUT_LINE')))
	{
		if (connector.outputQuantity == -1 || connector.outputQuantity > 0)
		{
			myState.dragging = true;
			var line = new Line(connector.x, connector.y, connector.x, connector.y, 'black', myState.onlyOutputLine);
			myState.AddLine(line);				
			myState.selection = line;
			myState.valid = false;
			
			myState.beginSelection = line.Clone();
			myState.isNewLine = true;
			
			result = true;
		}
	}
	
	return result;
}

CanvasState.prototype.ContainsLineOnMouseDown = function(mx, my){
	var myState = this;
	var lines = myState.lines;
	let maxLines = myState.lines.length;
	let lineSelected = false;
	
	for (var i = maxLines-1; i >= 0; i--)
	{
		lines[i].polygonSelector = null;	  
		if (lines[i].Contains(mx, my))
		{
			var line = lines[i];		
			myState.selection = line;		
			myState.dragoffx = mx - line.x;
			myState.dragoffy = my - line.y;
			myState.dragging = true;		
			myState.valid = false;

			myState.selection.polygonSelector = new PolygonSelector([
				{x : line.x, y : line.y, width: myState.selection.lineWidth},
				{x : line.x, y : line.y, width: myState.selection.lineWidth}], 
				myState.selection.type);

			myState.beginSelection = line.Clone();

			if (myState.selection.onlyOutputDirection) {
				myState.selection.directionMove = 1;
			}
			else {
				myState.selection.GetDirectionToMove(myState.beginMouseX, myState.beginMouseY);
			}

			// if (myState.selection.GetDirectionToMove(myState.beginMouseX, myState.beginMouseY) == 0
				// && myState.selection.onlyOutputDirection)
			// {
				// myState.selection.onlyOutputDirection;
				// lineSelected = false;
			// }
			// else
			// {
				lineSelected = true;
			// }
			
			myState.isNewLine = false;
			// break;
		}
	}
	
	return lineSelected;
}

CanvasState.prototype.GetLinesFromShape = function(shape, mx, my){
	var myState = this;
	const margin = 5;	
	
	if (shape && myState.lines && myState.lines.length > 0){		
		let lines = myState.lines;
		let maxLines = myState.lines.length;
		let bmx = mx; 
		let bmy = my;

		for (var countLines = maxLines-1; countLines >= 0; countLines--) {
			if (shape.ContainsWithMargin(lines[countLines].beginX, lines[countLines].beginY, margin)) {				
				myState.beginShapeLines.push(myState.lines[countLines].Clone());
				
				let distanceLine = ShapeService.GetDistanceLine(bmx, bmy, mx, my);
				let angleLine = ShapeService.GetRadianAngle(bmx, bmy, mx, my);						
				myState.lines[countLines].beginX += (distanceLine * Math.cos(angleLine));
				myState.lines[countLines].beginY += (distanceLine * Math.sin(angleLine));
				myState.shapeLines.push({line: myState.lines[countLines], type: 0});
			}
			else if (shape.ContainsWithMargin(lines[countLines].endX, lines[countLines].endY, margin)) {
				myState.beginShapeLines.push(myState.lines[countLines].Clone());
				
				let distanceLine = ShapeService.GetDistanceLine(bmx, bmy, mx, my);
				let angleLine = ShapeService.GetRadianAngle(bmx, bmy, mx, my);						
				myState.lines[countLines].endX += (distanceLine * Math.cos(angleLine));
				myState.lines[countLines].endY += (distanceLine * Math.sin(angleLine));			
				myState.shapeLines.push({line: myState.lines[countLines], type: 1});
			}
		}		
	}
}

CanvasState.prototype.MoveLineOnShape = function(bmx, bmy, mx, my){
	var myState = this;
	
	if (myState.shapeLines && myState.shapeLines.length > 0)
	{
		const margin = 5;
		let lines = []; 
		let maxLines = 0;
		lines = myState.shapeLines;
		maxLines = myState.shapeLines.length;
		for (var countLines = maxLines-1; countLines >= 0; countLines--) {
			var line = lines[countLines].line;
			if (lines[countLines].type == 0) {
				let distanceLine = ShapeService.GetDistanceLine(bmx, bmy, mx, my);
				let angleLine = ShapeService.GetRadianAngle(bmx, bmy, mx, my);				
				line.beginX += (distanceLine * Math.cos(angleLine));
				line.beginY += (distanceLine * Math.sin(angleLine));
			}
			else {
				let distanceLine = ShapeService.GetDistanceLine(bmx, bmy, mx, my);
				let angleLine = ShapeService.GetRadianAngle(bmx, bmy, mx, my);				
				line.endX += (distanceLine * Math.cos(angleLine));
				line.endY += (distanceLine * Math.sin(angleLine));
			}
		}
	}
}

CanvasState.prototype.CopyMultiple = function(){
	var myState = this;
	const margin = 5;
	myState.copySelection = {};
	myState.copySelection.rect = { 
		x : myState.screenShot.minX - margin,
		y : myState.screenShot.minY - margin,
		w : myState.screenShot.dWidth + (margin * 2),
		h : myState.screenShot.dHeight + (margin * 2)
	};
		
	var shapes = [];	
	for(let count = 0; count < myState.multipleSelection.length; count++){	
		if (myState.multipleSelection[count].full == true){
			shapes.push(myState.multipleSelection[count].shape.Copy());
		}
	}
	myState.copySelection.shapes = shapes;
}

CanvasState.prototype.PasteMultiple = function(){
	var myState = this;
	
	if (myState.copySelection){
		var minX = myState.copySelection.rect.x || 0;
		var minY = myState.copySelection.rect.y || 0;
		var mx = myState.beginMouseX;
		var my = myState.beginMouseY;
		
		var shapes = myState.copySelection.shapes;		
		if (shapes && shapes.length > 0){
			let l = shapes.length;
			
			for (var i = l-1; i >= 0; i--) {					
				var shape = shapes[i];
				
				switch(shape.type){
					case ShapeTypeEnum[ShapeTypeEnum.LINE]: {
						var line = shape;					
						line.beginX += mx - minX;
						line.beginY += my - minY;
						line.endX += mx - minX;
						line.endY += my - minY;					
						myState.PositionLineInGrid(line, mx, my, true, true, true);
					}
					break;
					default:{
						shape.x += mx - minX;
						shape.y += my - minY;
					}
				}
			}
			
			myState.InsertShape(shapes);
		}
	}
}

CanvasState.prototype.CanvasResize = function(mx, my) {
	var myState = this;
	var elementParent = myState.canvas.parentElement;	
	myState.canvasResizeDraw = false;
	
	if (elementParent){
		const canvasWidth = myState.canvas.width;
		const canvasHeight = myState.canvas.height;
		const parentMargin = 40;
		const moveSlide = 100;
		const offsetX = canvasWidth - parentMargin;
		const offsetY = canvasHeight - parentMargin;		
		myState.canvasResizeCoordinates.drawX = false;
		myState.canvasResizeCoordinates.drawY = false;
		if (mx > offsetX){	
			myState.canvasResizeCoordinates.x = offsetX;
			myState.canvasResizeCoordinates.y = elementParent.scrollTop;
			myState.canvasResizeCoordinates.w = parentMargin;
			myState.canvasResizeCoordinates.h = elementParent.offsetHeight;
			myState.canvasResizeDraw = true;
			myState.canvasResizeCoordinates.drawX = true;
		}
		
		if (my > offsetY){			
			myState.canvasResizeCoordinates.x = elementParent.scrollLeft;
			myState.canvasResizeCoordinates.y = offsetY;
			myState.canvasResizeCoordinates.w = elementParent.offsetWidth;
			myState.canvasResizeCoordinates.h = parentMargin;
			myState.canvasResizeDraw = true;
			myState.canvasResizeCoordinates.drawY = true;
		}
	}
}

CanvasState.prototype.CanvasSlide = function(mx, my){	
	var myState = this;	
	var elementParent = myState.canvas.parentElement;	
	
	if (elementParent){
		const canvasWidth = myState.canvas.width;
		const canvasHeight = myState.canvas.height;
		const parentMargin = 40;
		const moveSlide = 2;
				
		var offsetX = elementParent.offsetWidth + elementParent.scrollLeft - parentMargin;
		if (mx > offsetX){
			elementParent.scrollLeft += parentMargin + moveSlide;
		}
		else if (elementParent.scrollLeft + parentMargin > mx){
			elementParent.scrollLeft -= parentMargin + moveSlide;
		}
		
		var offsetY = elementParent.offsetHeight + elementParent.scrollTop - parentMargin;
		if (my > offsetY){
			elementParent.scrollTop += parentMargin + moveSlide;
		}
		else if (elementParent.scrollTop + parentMargin > my){
			elementParent.scrollTop -= parentMargin + moveSlide;
		}
	}
}

CanvasState.prototype.MainLoop = function(myState){
	myState.Draw(); 
	requestAnimationFrame(function(){myState.MainLoop(myState);});
}

CanvasState.prototype.UpdateShape = function(shapeList, disableUndoRedo){
	if (shapeList){
		disableUndoRedo = disableUndoRedo || false;
		var myState = this;	
		var selection = [];
		
		if (Array.isArray(shapeList))
		{
			selection = shapeList;
		}
		else
		{
			selection.push(shapeList);
		}
				
		this.valid = true;
		var shapesUpdate = [];		
		for(let count = 0; count < selection.length; count++){
			
			switch(selection[count].type){
				case ShapeTypeEnum[ShapeTypeEnum.LINE]:{
					for (let countLine = 0; countLine < myState.lines.length; countLine++){
						if (selection[count].id == myState.lines[countLine].id){
							myState.lines[countLine].SetLine(selection[count].beginX, selection[count].beginY, selection[count].endX, selection[count].endY, selection[count].fill);
							break;
						}
					}
				}
				break;
				default:{					
					for (let countShape = 0; countShape < myState.shapes.length; countShape++){
						if (selection[count].id == myState.shapes[countShape].id){							
							if (selection[count].type == ShapeTypeEnum[ShapeTypeEnum.CIRCLE]){
								myState.shapes[countShape].SetPosition(selection[count].x, selection[count].y);
							}else{							
								myState.shapes[countShape].SetRectangle(selection[count].x, selection[count].y, selection[count].w, selection[count].h);
							}
							break;
						}
					}
				}
			}
			
			if (!disableUndoRedo){
				shapesUpdate.push(selection[count]);
			}
		}
		
		if (!disableUndoRedo){			
			myState.urManager.AddObject(UndoRedoActionEnum.UPDATE, shapesUpdate);
		}
		this.valid = false;
	}
}

CanvasState.prototype.InsertShape = function(shapeList, disableUndoRedo){	
	if (shapeList){
		disableUndoRedo = disableUndoRedo || false;
		var myState = this;	
		var selection = [];
		
		if (Array.isArray(shapeList))
		{
			selection = shapeList;
		}
		else
		{
			selection.push(shapeList);
		}	
				
		var shapesInserted = [];
		this.valid = true;
		for(let count = 0; count < selection.length; count++){
			
			switch(selection[count].type){
				case ShapeTypeEnum[ShapeTypeEnum.LINE]:{
					myState.AddLine(selection[count]);
				}
				break;
				case ShapeTypeEnum[ShapeTypeEnum.SQUARE]:
				case ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]:
				case ShapeTypeEnum[ShapeTypeEnum.CIRCLE]:
				case ShapeTypeEnum[ShapeTypeEnum.TRIANGLE]:
				{
					myState.AddShape(selection[count]);
				}
				break;
			}
			
			if (!disableUndoRedo){
				shapesInserted.push(selection[count]);
			}
		}
		
		if (!disableUndoRedo){			
			myState.urManager.AddObject(UndoRedoActionEnum.ADD, shapesInserted);
		}
		this.valid = false;
	}
}

CanvasState.prototype.DeleteShape = function(shapeList, disableUndoRedo){
	if (shapeList){
		disableUndoRedo = disableUndoRedo || false;		
		
		var myState = this;		
		var selection = [];
		if (Array.isArray(shapeList))
		{
			selection = shapeList;
		}else
		{
			selection.push(shapeList);
		}		
		
		if (selection && selection.length > 0){
			this.valid = true;
			var shapesDeletes = [];
			
			// DELETE SHAPES
			for(let count = 0; count < selection.length; count++){
				for(let countShape = myState.shapes.length - 1; countShape >= 0; countShape--){
					
					if (selection[count] ===  myState.shapes[countShape]){
						if (!disableUndoRedo){
							shapesDeletes.push(myState.shapes[countShape]);
						}
						
						myState.shapes.splice(countShape, 1);
					}
				}
			}
			
			// DELETE LINES
			for(let count = 0; count < selection.length; count++){
				for(let countShape = myState.lines.length - 1; countShape >= 0; countShape--){					
					if (selection[count] ===  myState.lines[countShape]){
						if (!disableUndoRedo){
							shapesDeletes.push(myState.lines[countShape]);
						}
						
						myState.lines.splice(countShape, 1);
					}
				}
			}
			
			if (!disableUndoRedo){
				myState.urManager.AddObject(UndoRedoActionEnum.DELETE, shapesDeletes);
			}
						
			this.valid = false;
		}
	}
}

CanvasState.prototype.PositionShapeInGrid = function(shape){
	if (!this.freeStyle && shape) {		
		const gridSize = this.gridSize;
		const margin = 20;
		this.valid = true;
		// let center = shape.GetCenter();
		
		var columnX = Math.round(shape.x / gridSize);
		var columnY = Math.round(shape.y / gridSize);			
		let x = margin + (columnX * gridSize);
		let y = margin + (columnY * gridSize);		
		shape.SetPosition(x, y);		
		// shape.ToGrid();
		
		this.valid = false;
		// this.ctx.stroke();
	}	
}

CanvasState.prototype.PositionLineInGrid = function(line, x, y, moveBegin, moveEnd, withScreenShot){
	
	if (!this.freeStyle && line && line.type == ShapeTypeEnum[ShapeTypeEnum.LINE]) {	
		const gridSize = this.gridSize;
		const margin = 20;
		const screenShotMargin = 5;
		
		if (withScreenShot){
			x = x  + screenShotMargin || 0;
			y = y  + screenShotMargin || 0;
		}
		else{
			x = x || 0;
			y = y || 0;			
		}
		
		moveBegin = moveBegin || false;
		moveEnd = moveEnd || false;
		
		this.valid = true;
		
		var columnX = Math.round(x / gridSize);
		var columnY = Math.round(y / gridSize);
		
		var dx = columnX * gridSize + margin;
		var dy = columnY * gridSize + margin;
		
		var angle = ShapeService.GetRadianAngle(x, y, dx, dy);
		var distanceShape = ShapeService.GetDistanceLine(x, y, dx, dy);
		
		if (moveBegin){
			line.beginX += (distanceShape * Math.cos(angle));
			line.beginY += (distanceShape * Math.sin(angle));
		}
		
		if (moveEnd){
			line.endX += (distanceShape * Math.cos(angle));
			line.endY += (distanceShape * Math.sin(angle));
		}
		
		this.valid = false;
		this.ctx.stroke();
	}
	
	if (line && line.type == ShapeTypeEnum[ShapeTypeEnum.LINE]){
		// this.urManager.AddObject(UndoRedoActionEnum.UPDATE, shape);
	}
}

// CanvasState.prototype.CropImage = function(src, width, height){
    // var crop = width == 0 || height == 0;
    // // not resize
    // if(src.width <= width && height == 0) {
        // width  = src.width;
        // height = src.height;
    // }
    // // resize
    // if( src.width > width && height == 0){
        // height = src.height * (width / src.width);
    // }

    // // check scale
    // var xscale = width  / src.width;
    // var yscale = height / src.height;
    // var scale  = crop ? Math.min(xscale, yscale) : Math.max(xscale, yscale);
    // // create empty canvas
    // var canvas = document.createElement("canvas");                  
    // canvas.width  = width ? width   : Math.round(src.width  * scale);
    // canvas.height = height ? height : Math.round(src.height * scale);
    // canvas.getContext("2d").scale(scale,scale);
    // // crop it top center
    // canvas.getContext("2d").drawImage(src, ((src.width * scale) - canvas.width) * -.5 , ((src.height * scale) - canvas.height) * -.5 );
    // return canvas;
// }

CanvasState.prototype.CropImage = function(fn, imageCaptured, sx, sy, sWidth, sHeight, dx, dy, dWidth, dHeight){
	if (fn){
		var imageResult = null;
		var canvas = document.createElement("canvas");
		canvas.width  = sWidth;
		canvas.height = sHeight;

		var img = new Image();
		img.src = imageCaptured;
		
		img.onload = function(e){
			canvas.getContext("2d").drawImage(img, sx, sy, sWidth, sHeight, dx, dy, dWidth, dHeight);	
			// imageResult = canvas.toDataURL("image/jpeg", 0.5);
			imageResult = canvas.toDataURL("image/png", 0.5);
			fn(imageResult);
		}
	}
}

CanvasState.prototype.CaptureScreen = function(imageCaptured, sx, sy, sWidth, sHeight, dx, dy, dWidth, dHeight, crop){
	var myState = this;
	crop = crop || false;
	
	var imageResult = null;
	
	if (crop){
		imageResult = myState.CropImage(function(result){
			myState.screenShot = {
			show: false,
			image: result,
			sx: crop ? 0 : sx,
			sy: crop ? 0 : sy,
			sWidth: sWidth, 
			sHeight: sHeight, 
			dx: dx, 
			dy: dy, 
			dWidth: dWidth,
			dHeight: dHeight,
			minX: sx,
			minY: sy,
			crop: crop
		};
		},
		imageCaptured, sx, sy, sWidth, sHeight, dx, dy, dWidth, dHeight);
	}else{
		imageResult = imageCaptured;
		
		myState.screenShot = {
			show: false,
			image: imageResult,
			sx: crop ? 0 : sx,
			sy: crop ? 0 : sy,
			sWidth: sWidth, 
			sHeight: sHeight, 
			dx: dx, 
			dy: dy, 
			dWidth: dWidth,
			dHeight: dHeight,
			minX: sx,
			minY: sy,
			crop: crop
		};
	}
}

CanvasState.prototype.ContainsLineOnShape = function(mx, my){
	const margin = 5;
	var shape = null;
	var myState = this
	var shapes = myState.shapes;
	var l = shapes.length;	
	for (var i = l-1; i >= 0; i--) {			
		var mySel = shapes[i];
		if (shapes[i].ContainsWithMargin(mx, my, margin)) {
			shape = shapes[i];
			break;
		}
	}
	
	return shape;
}

CanvasState.prototype.ContainsLine = function(mx, my){
	var result = false;
	var myState = this;
	var lines = myState.lines;
	var l = myState.lines.length;
	for (var i = l-1; i >= 0; i--) {
		if (lines[i].Contains(mx, my)) {
			result = lines[i];
			break;
		}
	}
	
	return result;
}

CanvasState.prototype.ContainsConnector = function(mx, my, type){
	var connectorProperties = null;
	var myState = this;
	var shapes = myState.shapes;
	var l = shapes.length;
	for (var i = l-1; i >= 0; i--) {
	  if (shapes[i].polygonConnector){			
		  if (shapes[i].polygonConnector.Contains(mx, my, type)) {
			  var connector = shapes[i].polygonConnector.connectorSelected;			  
			  connectorProperties = {
					x: connector.x,
					y: connector.y,
					radius: connector.radius,
					isVertice: connector.isVertice,
					direction: connector.direction,
					inputQuantity: connector.inputQuantity,
					outputQuantity: connector.outputQuantity
				};
			  break;
		  }
	  }
	}
	return connectorProperties;
}

CanvasState.prototype.GetConnectorByCoordinate = function(mx, my, type){
	var connector = null;	
	var myState = this;
	var shapes = myState.shapes;
	var l = shapes.length;
	for (var i = l-1; i >= 0; i--) {
	  if (shapes[i].polygonConnector){			
		  if (shapes[i].polygonConnector.Contains(mx, my, type)) {			  
			  connector = shapes[i].polygonConnector.connectorSelected;
			  break;
		  }
	  }
	}
	return connector;
}

CanvasState.prototype.DeleteLineByConnector = function(connector) {
	var myState = this;
	var maxLines = myState.lines.length;
	if (connector && connector.obsoleteLine){
		for (var countLines = maxLines-1; countLines >= 0; countLines--){
			if (myState.lines[countLines] == connector.obsoleteLine){
				myState.lines.splice(countLines, 1);
				connector.obsoleteLine = null;
				break;
			}
		}
	}
}

CanvasState.prototype.DeleteLine = function(lineDelete){
	var result = false;
	var myState = this;
	if (myState.lines && myState.lines.length > 0){
		let maxLines = myState.lines.length;
		for (var countLines = maxLines-1; countLines >= 0; countLines--){
			let line = myState.lines[countLines];
			
			if (lineDelete === line){
				myState.lines.splice(countLines, 1);
				result = true;
				break;
			}
		}
	}
	return result;
}

CanvasState.prototype.AddShape = function(shape) {	
  this.PositionShapeInGrid(shape);
  
  if (shape.polygonConnector){
	  shape.polygonConnector.SetVertices([{
				x : shape.x,
				y : shape.y,
				width : shape.w,
				height : shape.h,
				radius: shape.radius
			}]);
  }
		
  this.shapes.push(shape);  
  this.valid = false;
}

CanvasState.prototype.AddLine = function(line) {
  this.lines.push(line);
  this.valid = false;
}

CanvasState.prototype.Clear = function() {
  this.ctx.clearRect(0, 0, this.width, this.height);
}

// While draw is called as often as the INTERVAL variable demands,
// It only ever does something if the canvas gets invalidated by our code
CanvasState.prototype.Draw = function() {
  // if our state is invalid, redraw and validate!
  if (!this.valid) {
    var ctx = this.ctx;
    var shapes = this.shapes;
	var lines = this.lines;
	// var polygonSelectors = this.polygonSelectors;
    this.Clear();
    
    // ** Add stuff you want drawn in the background all the time here **
    
    // draw all shapes
    var l = shapes.length;
    for (var i = 0; i < l; i++) {
      var shape = shapes[i];
      // We can skip the drawing of elements that have moved off the screen:
      if (shape.x > this.width || shape.y > this.height ||
          shape.x + shape.w < 0 || shape.y + shape.h < 0) continue;
      shapes[i].Draw(ctx);
    }
    
    // draw selection
    // right now this is just a stroke along the edge of the selected Shape
    if (this.selection != null) {
		
    }
    
    // ** Add stuff you want drawn on top all the time here **	
	l = lines.length;
    for (var i = 0; i < l; i++) {
      var line = lines[i];
      lines[i].Draw(ctx);
    }
	
	DrawSelector.Draw(ctx);
	
	var screenShot = this.screenShot;
	if (screenShot.show){
		var image = new Image();
		image.src = screenShot.image;
		ctx.globalAlpha = 0.5;
		// image.onload = function(e){
			ctx.drawImage(image, screenShot.crop ? 0 : screenShot.sx, screenShot.crop ? 0 : screenShot.sy, 
				screenShot.sWidth, screenShot.sHeight, screenShot.dx, screenShot.dy, screenShot.dWidth, screenShot.dHeight);
		// }
		
		ctx.globalAlpha = 1;
	}
	
	if (this.canvasResizeDraw){
		var coordinates = this.canvasResizeCoordinates;			
		ctx.beginPath();		
		ctx.fillStyle = 'gray';
		ctx.strokeStyle = 'black';
		ctx.lineWidth = 3;
		ctx.rect(coordinates.x, coordinates.y, coordinates.w, coordinates.h);
		ctx.globalAlpha = 0.8;
		ctx.stroke();
		ctx.fill();	
		
		var points = [];
		const triangleSize = 70;
		if (coordinates.drawX){
			points.push( {x: coordinates.x, y: coordinates.y + (coordinates.h / 2) - triangleSize} );
			points.push( {x: coordinates.x + coordinates.w, y: coordinates.y + coordinates.h / 2} );
			points.push( {x: coordinates.x, y: coordinates.y + (coordinates.h / 2) + triangleSize} );
		}else if (coordinates.drawY){
			points.push( {x: coordinates.x + (coordinates.w / 2) - triangleSize, y: coordinates.y} );
			points.push( {x: coordinates.x + coordinates.w / 2, y: coordinates.y + coordinates.h} );
			points.push( {x: coordinates.x + (coordinates.w / 2) + triangleSize, y: coordinates.y} );
		}
		
		ctx.fillStyle = 'orange';
		ctx.strokeStyle = 'black';
		ctx.globalAlpha = 1.0;
		ShapeService.DrawTriangle(ctx, points);
		
		ctx.lineWidth = 1;
		ctx.globalAlpha = 1;
	}
	
	if (this.toolbar){
		this.toolbar.Draw(ctx);
	}
    
    this.valid = true;
  }
}


// Creates an object with x and y defined, set to the mouse position relative to the state's canvas
// If you wanna be super-correct this can be tricky, we have to worry about padding and borders
CanvasState.prototype.GetMouse = function(e) {
  var element = this.canvas, offsetX = 0, offsetY = 0, mx, my;
  var elementParent = this.canvas.parentElement;
  
  // Compute the total offset
  if (element.offsetParent !== undefined) {
    do {
      offsetX += element.offsetLeft;
      offsetY += element.offsetTop;
    } while ((element = element.offsetParent));
  }
  
  if (elementParent !== undefined && elementParent.tagName.toUpperCase() == "DIV") {
      offsetX -= elementParent.scrollLeft;
      offsetY -= elementParent.scrollTop;
  }

  // Add padding and border style widths to offset
  // Also add the <html> offsets in case there's a position:fixed bar
  offsetX += this.stylePaddingLeft + this.styleBorderLeft + this.htmlLeft;
  offsetY += this.stylePaddingTop + this.styleBorderTop + this.htmlTop;

  mx = e.pageX - offsetX;
  my = e.pageY - offsetY;
  
  // We return a simple javascript object (a hash) with x and y defined
  return {x: mx, y: my};
}

CanvasState.prototype.Insert = function(){
	var myState = this;
	
	var shape = null;
	if (myState.shapeTypeSelected){	
		switch(myState.shapeTypeSelected) {
			case ShapeTypeEnum[ShapeTypeEnum.LINE]: {
				shape = new Line(mx, my, mx + 20, my, 'black', myState.onlyOutputLine)
			}
			break;
			case ShapeTypeEnum[ShapeTypeEnum.RECTANGLE]: {
				var mx = myState.beginMouseX;
				var my = myState.beginMouseY;
				shape = new Rectangle(mx, my, 60, 60, 'lightskyblue');
			}
			break;
		}
	}
		
	this.InsertShape(shape);
}

CanvasState.prototype.Delete = function(){
	var myState = this;
	
	var shapes = null;
	if (myState.selection){
		shapes = [];
		shapes.push(myState.selection);		
		if (myState.shapeLines && myState.shapeLines.length > 0){
			for(let count = 0; count < myState.shapeLines.length; count++){
				shapes.push(myState.shapeLines[count].line);				
			}
		}
	}
	else if(myState.multipleSelection && myState.multipleSelection.length > 0){		
		shapes = []
		for(let count = 0; count < myState.multipleSelection.length; count++){
			shapes.push(myState.multipleSelection[count].shape);
		}
	}
	
	this.DeleteShape(shapes);
}

CanvasState.prototype.Save = function(){
	var myState = this;
	
	var shapesSave = [];
	if (myState.shapes != null && myState.shapes.length > 0) {
		for(let count = 0; count < myState.shapes.length; count++) {
			var shape = myState.shapes[count];
			shapesSave.push(shape.GetProperties());
		}
	}
	
	if (myState.lines != null && myState.lines.length > 0) {
		for(let count = 0; count < myState.lines.length; count++) {
			var line = myState.shapes[count];
			shapesSave.push(line.GetProperties());
		}
	}
	
	alert(shapesSave);
	// alert(JSON.stringify(shapesSave));
}

CanvasState.prototype.Copy = function(){
	this.CopyMultiple();
}

CanvasState.prototype.Paste = function() {
	var myState = this;
	
	if (myState.copySelection.shapes && myState.copySelection.shapes.length > 0){
		myState.PasteMultiple();
		myState.copySelection = {};
	}
}

CanvasState.prototype.ProcessUndoRedo = function(reverseAction, object){
	if (reverseAction){
		switch (reverseAction){
			case UndoRedoActionEnum.ADD:
			{
				this.InsertShape(object, true);
			}
				break;
			case UndoRedoActionEnum.RESTORE:
			{
				this.UpdateShape(object, true);
			}
				break;
			case UndoRedoActionEnum.UPDATE:
			{
				this.UpdateShape(object, true);
			}
				break;
			case UndoRedoActionEnum.DELETE:
			{				
				this.DeleteShape(object, true);
			}
				break;
			case UndoRedoActionEnum.SELECT:
			{
			}
				break;
			case UndoRedoActionEnum.UNSELECT:
			{
			}
				break;
		}
	}
}

CanvasState.prototype.Undo = function(){
	var myState = this;	
	this.urManager.Undo(function(reverseAction, object, inverseValue){	
		myState.ProcessUndoRedo(reverseAction, object, inverseValue);
	});
}

CanvasState.prototype.Redo = function(){
	var myState = this;
	this.urManager.Redo(function(reverseAction, object, inverseValue){		
		myState.ProcessUndoRedo(reverseAction, object, inverseValue);
	});
}

// If you dont want to use <body onLoad='init()'>
// You could uncomment this init() reference and place the script reference inside the body tag
//init();

function init() {
							 // canvas, 							freeStyle, 	freeLines, 	onlyOutputLine, gridSize
  canvasState = new CanvasState(document.getElementById('canvas1'), false, 		false, 		true);
  canvasState.InsertShape(new Rectangle(90,90,60,60, 'lightskyblue'));
  canvasState.InsertShape(new Rectangle(90,160,60,60, 'lightskyblue'));
  canvasState.InsertShape(new Rectangle(300,90,60,60, 'lightskyblue'));
  canvasState.InsertShape(new Rectangle(300,160,60,60, 'lightskyblue'));
}

// Now go make something amazing!
